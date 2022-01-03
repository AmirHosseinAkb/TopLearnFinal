using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TopLearn.Core.Services.Interfaces;
using TopLearn.Core.DTOs.User;

namespace TopLearn.Pages.Admin.Users
{
    public class DeleteUserModel : PageModel
    {
        private IUserService _userService;
        public DeleteUserModel(IUserService userService)
        {
            _userService = userService;
        }
        [BindProperty]
        public UserInformationsForShowViewModel UserInformationsForShowViewModel { get; set; }
        public void OnGet(int userId)
        {
            ViewData["UserId"] = userId;
            UserInformationsForShowViewModel = _userService.GetUserInformationsForShow(userId);
        }

        public IActionResult OnPost(int userId)
        {
            _userService.DeleteUserFromAdmin(userId);
            return RedirectToPage("Index");
        }
    }
}
