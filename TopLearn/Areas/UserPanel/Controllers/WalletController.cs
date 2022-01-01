using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using TopLearn.Core.DTOs.User;
using TopLearn.Core.Services.Interfaces;
using TopLearn.Data.Entities.Wallet;

namespace TopLearn.Areas.UserPanel.Controllers
{
    [Authorize]
    [Area("UserPanel")]
    public class WalletController : Controller
    {
        private IUserService _userService;

        public WalletController(IUserService userService)
        {
            _userService = userService;
        }

        public IActionResult Index()
        {
            ViewBag.UserWallets = _userService.GetUserWallets(User.Identity.Name);
            return View();
        }

        [HttpPost]
        [Route("ChargeWallet")]
        public IActionResult ChargeWallet(WalletViewModel walletModel)
        {
            if (!ModelState.IsValid)
                return View("Index", walletModel);
            if (walletModel.Amount < 5000)
            {
                ModelState.AddModelError("Amount","حداقل مبلغ پرداختی 5000 تومان است");
                return View("Index", walletModel);
            }

            var userId = _userService.GetUserIdByUserName(User.Identity.Name);
            //AddWallet
            Wallet wallet = new Wallet()
            {
                UserId = userId,
                TypeId = 1,
                Amount = walletModel.Amount,
                Description = "شارژ کیف پول",
                CreateDate = DateTime.Now,
                IsPayed = false
            };
            int walletId = _userService.AddWallet(wallet);

            //ZarinPal
            var payment = new ZarinpalSandbox.Payment(walletModel.Amount);
            var response = payment.PaymentRequest("شارژ کیف پول", "http://localhost:46358/OnlinePayment/" + walletId);
            if (response.Result.Status == 100)
            {
                return Redirect("https://SandBox.ZarinPal.Com/pg/StartPay/" + response.Result.Authority);
            }

            return BadRequest();
        }
    }
}
