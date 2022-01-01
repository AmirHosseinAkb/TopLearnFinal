using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TopLearn.Core.Services.Interfaces;

namespace TopLearn.Controllers
{
    public class HomeController : Controller
    {
        private IUserService _userService;

        public HomeController(IUserService userService)
        {
            _userService = userService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [Route("OnlinePayment/{walletId}")]
        public IActionResult OnlinePayment(int walletId)
        {
            if (HttpContext.Request.Query["Status"] != ""
                && HttpContext.Request.Query["Status"].ToString().ToLower() == "ok"
                && HttpContext.Request.Query["Authority"] != "")
            {
                var authority = HttpContext.Request.Query["Authority"];
                var wallet = _userService.GetWalletByWalletId(walletId);

                var payment = new ZarinpalSandbox.Payment(wallet.Amount);
                var response = payment.Verification(authority).Result;
                if (response.Status == 100)
                {
                    wallet.IsPayed = true;
                    _userService.UpdateWallet(wallet);
                    ViewBag.IsPayed = true;
                    ViewBag.Code = response.RefId;
                }
            }

            return View();
        }
    }
}
