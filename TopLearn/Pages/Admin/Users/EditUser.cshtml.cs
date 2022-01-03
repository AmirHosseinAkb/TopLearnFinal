using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TopLearn.Core.Services.Interfaces;
using TopLearn.Core.DTOs.User;
using TopLearn.Core.Security;
using TopLearn.Core.Convertors;

namespace TopLearn.Pages.Admin.Users
{
    public class EditUserModel : PageModel
    {
        private IUserService _userService;
        private IPermissionService _permissionService;
        public EditUserModel(IUserService userService,IPermissionService permissionService)
        {
            _userService = userService;
            _permissionService = permissionService;
        }
        [BindProperty]
        public EditUserViewModel EditUserViewModel { get; set; }
        public void OnGet(int userId)
        {
            ViewData["UserRoles"] = _permissionService.GetUserRoles(userId);
            ViewData["Roles"] = _permissionService.GetAllRoles();
            EditUserViewModel = _userService.GetUserForEditFromAdmin(userId);
        }

        public IActionResult OnPost(List<int> SelectedRoles)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Roles"] = _permissionService.GetAllRoles();
                return Page();
            }
            var user = _userService.GetUserById(EditUserViewModel.UserId);

            if (user.Email != EmailConvertor.FixEmail(EditUserViewModel.Email))
            {
                if (_userService.IsExistUserByEmail(EditUserViewModel.Email))
                {
                    ViewData["Roles"] = _permissionService.GetAllRoles();
                    ViewData["ExistEmail"] = true;
                    return Page();
                }
            }
            if (!EditUserViewModel.UserAvatar.IsImage())
            {
                ViewData["Roles"] = _permissionService.GetAllRoles();
                ViewData["IsntImage"] = true;
                return Page();
            }

            //Edit User
            _userService.EditUserFromAdmin(EditUserViewModel);
            //Edit UserRoles
            _permissionService.EditUserRoles(EditUserViewModel.UserId, SelectedRoles);
            return RedirectToPage("Index");
        }
    }
}
