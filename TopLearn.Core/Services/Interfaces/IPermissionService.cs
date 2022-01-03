using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopLearn.Data.Entities.Permission;
using TopLearn.Data.Entities.User;

namespace TopLearn.Core.Services.Interfaces
{
    public interface IPermissionService
    {
        List<Role> GetAllRoles();
        void AddUserRoles(int userId, List<int> roleIds);
        List<int> GetUserRoles(int userId);
        void EditUserRoles(int userId, List<int> roleIds);
        bool IsExistRole(string roleTitle);
        int AddRole(Role role);
        void AddRolePermissions(int roleId, List<int> permissionIds);
        List<Permission> GetAllPermissions();
        List<int> GetRolePermissions(int roleId);
        Role GetRoleById(int roleId);
        Role GetRoleByIdNoTracking(int roleId);
        void UpdateRole(Role role);
        void EditRolePermissions(int roleId, List<int> permissionIds);
    }
}
