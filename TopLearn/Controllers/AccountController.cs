using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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

        [Route("Login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [Route("Login")]
        public IActionResult Login(LoginUserViewModel login)
        {
            if (!ModelState.IsValid)
                return View(login);

            var user = _userService.GetUserForLogin(login.Email, login.Password);
            if (user != null)
            {
                if (user.IsActive)
                {
                    List<Claim> claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                        new Claim(ClaimTypes.Name, user.UserName)
                    };
                    var identity = new ClaimsIdentity(claims,CookieAuthenticationDefaults.AuthenticationScheme);

                    var principal = new ClaimsPrincipal(identity);
                    var properties = new AuthenticationProperties()
                    {
                        IsPersistent = login.RememberMe
                    };
                    HttpContext.SignInAsync(principal, properties);
                    ViewBag.IsLogedIn = true;
                    return View();
                }
                else
                {
                    ModelState.AddModelError("Password","حساب کاربری شما غیرفعال است");
                    return View(login);
                }
            }
            else
            {
                ModelState.AddModelError("Password","کاربری با این مشخصات وجود ندارد");
                return View(login);
            }
            
        }

        [Route("ForgotPassword")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [Route("ForgotPassword")]
        [HttpPost]
        public IActionResult ForgotPassword(ForgotPasswordViewModel forgot)
        {
            var user = _userService.GetUserByEmail(forgot.Email);
            if (user == null || !user.IsActive)
            {
                ModelState.AddModelError("Email","کاربری با ایمیل وارد شده وجود ندراد");
                return View(forgot);
            }

            //SendEmail
            string body = _renderService.RenderToStringAsync("_ForgotPasswordEmailBody", user);
            SendEmail.Send(user.Email,"بازیابی رمز عبور|تاپلرن",body);
            ViewBag.IsEmailSent = true;
            return View();
        }

        [Route("ChangePassword/{activeCode?}")]
        public IActionResult ChangePassword(string activeCode)
        {
            var user = _userService.GetUserByActiveCode(activeCode);
            ChangePasswordViewModel change = new ChangePasswordViewModel()
            {
                ActiveCode = activeCode
            };
            if (user == null)
            {
                ViewBag.IsExpierd = true;
                return View();
            }
            return View(change);
        }

        [Route("ChangePassword/{activeCode?}")]
        [HttpPost]

        public IActionResult ChangePassword(ChangePasswordViewModel change)
        {
            if (!ModelState.IsValid)
                return View(change);
            var user = _userService.GetUserByActiveCode(change.ActiveCode);
            if (user == null || !user.IsActive)
            {
                return Forbid();
            }

            user.Password = PasswordHasher.HashPasswordMD5(change.Password);
            user.ActiveCode = NameGenerator.GenerateUniqCode();
            _userService.UpdateUser(user);
            ViewBag.IsChanged = true;
            return View();
        }
    }
}



