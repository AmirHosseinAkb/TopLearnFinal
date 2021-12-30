using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TopLearn.Core.Services.Interfaces;
using TopLearn.Core.DTOs.User;
using TopLearn.Data.Entities.User;
using TopLearn.Core.Convertors;
using TopLearn.Core.Generators;
using TopLearn.Core.Security;
using TopLearn.Core.Senders;

namespace TopLearn.Controllers
{
    public class AccountController : Controller
    {
        private IUserService _userService;
        private IViewRenderService _renderService;

        public AccountController(IUserService userService, IViewRenderService renderService)
        {
            _userService = userService;
            _renderService = renderService; 
        }


        [Route("Register")]
        public IActionResult Register()
        {
            return View();
        }
        [Route("Register")]
        [HttpPost]

        public IActionResult Register(RegisterUserViewModel register)
        {
            if (!ModelState.IsValid)
                return View(register);

            if (_userService.IsExistUserByUserName(register.UserName))
            {
                ModelState.AddModelError("UserName", "این نام کاربری از قبل وجود دارد");
                return View(register);
            }
            if (_userService.IsExistUserByEmail(register.Email))
            {
                ModelState.AddModelError("Email", "این ایمیل از قبل وجود دارد");
                return View(register);
            }

            TopLearn.Data.Entities.User.User user = new User()
            {
                UserName = register.UserName,
                Email = EmailConvertor.FixEmail(register.Email),
                Password = PasswordHasher.HashPasswordMD5(register.Password),
                IsActive = false,
                ActiveCode = NameGenerator.GenerateUniqCode(),
                AvatarName = "Default.png",
                IsDeleted = false,
                RegisterDate = DateTime.Now
            };
            //Add User
            _userService.AddUser(user);
            //Send Email
            string body = _renderService.RenderToStringAsync("_ActivationEmailBody", user);
            SendEmail.Send(user.Email,"فعالسازی حساب کابری|تاپلرن",body);
            return View("_SuccessRegister",user);
        }

        [Route("ActiveAccount/{activeCode}")]
        public IActionResult ActiveAccount(string activeCode)
        {
            ViewBag.IsActived = _userService.ActiveAccount(activeCode);
            return View();
        }
    }
}



