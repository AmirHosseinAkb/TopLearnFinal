using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TopLearn.Core.Services.Interfaces;
using TopLearn.Data.Entities.User;

namespace TopLearn.Pages.Admin.Users
{
    public class ReturnUserModel : PageModel
    {
        private IUserService _userService;
        public ReturnUserModel(IUserService userService)
        {
            _userService = userService;
        }
        public IActionResult OnGet(int userId)
        {
            _userService.ReturnUserFromDeletedUsers(userId);
            return RedirectToPage("Index");
        }

    }
}
