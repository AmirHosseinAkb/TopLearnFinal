using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TopLearn.Core.Services.Interfaces;
using TopLearn.Data.Entities.User;

namespace TopLearn.Pages.Admin.Roles
{
    public class EditRoleModel : PageModel
    {
        private IUserService _userService;
        private IPermissionService _permissionService;
        public EditRoleModel(IUserService userService,IPermissionService permissionService)
        {
            _userService = userService;
            _permissionService = permissionService;
        }

        public void GetInformations(int roleId)
        {
            ViewData["Permissions"] = _permissionService.GetAllPermissions();
            ViewData["RolePermissions"] = _permissionService.GetRolePermissions(roleId);
        }
        [BindProperty]
        public Role Role { get; set; }
        public void OnGet(int roleId)
        {
            GetInformations(roleId);
            Role = _permissionService.GetRoleById(roleId);
        }

        public IActionResult OnPost(List<int> SelectedPermissions)
        {
            if(!ModelState.IsValid)
            {
                GetInformations(Role.RoleId);
                return Page();
            }
            var role = _permissionService.GetRoleByIdNoTracking(Role.RoleId);
            if (Role.RoleTitle != role.RoleTitle)
            {
                if (_permissionService.IsExistRole(Role.RoleTitle))
                {
                    GetInformations(Role.RoleId);
                    ViewData["ExistRole"] = true;
                    return Page();
                }
            }
            //Edit Role
            _permissionService.UpdateRole(Role);
            //Edit Role Permissions
            _permissionService.EditRolePermissions(Role.RoleId, SelectedPermissions);
            return RedirectToPage("Index");
        }
    }
}
