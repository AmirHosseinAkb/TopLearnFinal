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
    public class IndexModel : PageModel
    {
        private IUserService _userService;
        private IPermissionService _permissionService;
        public IndexModel(IUserService userService,IPermissionService permissionService)
        {
            _userService = userService;
            _permissionService = permissionService;
        }
        public List<Role> Roles { get; set; }
        public void OnGet()
        {
            Roles = _permissionService.GetAllRoles();
        }
    }
}
