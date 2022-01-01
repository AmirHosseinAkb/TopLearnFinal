using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using TopLearn.Core.Convertors;
using TopLearn.Core.DTOs.User;
using TopLearn.Core.Security;
using TopLearn.Core.Services.Interfaces;

namespace TopLearn.Areas.UserPanel.Controllers
{
    [Authorize]
    [Area("UserPanel")]
    public class HomeController : Controller
    {
        private IUserService _userService;

        public HomeController(IUserService userService)
        {
            _userService = userService;
        }
        public IActionResult Index()
        {
            var user = _userService.GetUserInformationsForShow(User.Identity.Name);
            return View(user);
        }

        [Route("UserPanel/EditProfile")]

        public IActionResult EditProfile()
        {
            return View(_userService.GetUserForEdit(User.Identity.Name));
        }

        [Route("UserPanel/EditProfile")]
        [HttpPost]
        public IActionResult EditProfile(EditUserProfileViewModel edit)
        {
            if (!ModelState.IsValid)
                return View(edit);
            if (!edit.UserAvatar.IsImage())
            {
                ModelState.AddModelError("UserAvatar","لطفا فقط تصویر انتخاب کنید");
                return View(edit);
            }
            var user = _userService.GetUserByUserName(User.Identity.Name);
            if (EmailConvertor.FixEmail(edit.Email) != user.Email)
            {
                if (_userService.IsExistUserByEmail(edit.Email))
                {
                    ModelState.AddModelError("Email","این ایمیل از قبل وجود دارد");
                    return View(edit);
                }
            }
            _userService.EditUser(User.Identity.Name,edit);
            return Redirect("/Login?isEdited=true");
        }

        [Route("UserPanel/ResetPassword")]
        public IActionResult ResetPassword()
        {
            return View();
        }
        [HttpPost]
        [Route("UserPanel/ResetPassword")]
        public IActionResult ResetPassword(ResetPasswordViewModel resetPassword)
        {
            if (!ModelState.IsValid)
                return View();
            var user = _userService.GetUserByUserName(User.Identity.Name);
            if (user.Password != PasswordHasher.HashPasswordMD5(resetPassword.CurrentPassword))
            {
                ModelState.AddModelError("CurrentPassword","رمز عبور فعلی خود را بصورت صحیح وارد کنید");
                return View(resetPassword);
            }

            user.Password = PasswordHasher.HashPasswordMD5(resetPassword.NewPassword);
            _userService.UpdateUser(user);
            ViewBag.IsPasswordChanged = true;
            return View();
        }
    }
}
