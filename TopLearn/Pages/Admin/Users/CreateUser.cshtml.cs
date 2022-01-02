using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TopLearn.Core.Services.Interfaces;
using TopLearn.Core.DTOs.User;
using TopLearn.Core.Security;

namespace TopLearn.Pages.Admin.Users
{
    public class CreateUserModel : PageModel
    {
        private IUserService _userService;
        private IPermissionService _permissionService;

        public CreateUserModel(IUserService userService,IPermissionService permissionService)
        {
            _userService = userService;
            _permissionService = permissionService;
        }
        [BindProperty]
        public CreateUserViewModel CreateUserViewModel { get; set; }
        public void OnGet()
        {
            ViewData["Roles"] = _permissionService.GetAllRoles();
        }
        public IActionResult OnPost(List<int> SelectedRoles)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Roles"] = _permissionService.GetAllRoles();
                return Page();
            }
                

            if (_userService.IsExistUserByUserName(CreateUserViewModel.UserName))
            {
                ViewData["Roles"] = _permissionService.GetAllRoles();
                ViewData["ExistUserName"] = true;
                return Page();
            }
            if (_userService.IsExistUserByEmail(CreateUserViewModel.Email))
            {
                ViewData["Roles"] = _permissionService.GetAllRoles();
                ViewData["ExistEmail"] = true;
                return Page();
            }
            if (!CreateUserViewModel.UserAvatar.IsImage())
            {
                ViewData["Roles"] = _permissionService.GetAllRoles();
                ViewData["IsntImage"] = true;
                return Page();
            }
            //Add User
            int userId = _userService.AddUserFromAdmin(CreateUserViewModel);

            //AddUser Roles
            _permissionService.AddUserRoles(userId, SelectedRoles);
            return RedirectToPage("Index");
        }
    }
}
