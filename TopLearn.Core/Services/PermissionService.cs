using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopLearn.Core.Services.Interfaces;
using TopLearn.Data.Entities.User;
using TopLearn.Data.Context;
using TopLearn.Data.Entities.Permission;
using Microsoft.EntityFrameworkCore;

namespace TopLearn.Core.Services
{
    public class PermissionService : IPermissionService
    {
        private TopLearnContext _context;
        public PermissionService(TopLearnContext context)
        {
            _context = context;
        }

        public int AddRole(Role role)
        {
            _context.Roles.Add(role);
            _context.SaveChanges();
            return role.RoleId;
        }

        public void AddRolePermissions(int roleId, List<int> permissionIds)
        {
            foreach (var permissionId in permissionIds)
            {
                _context.RolePermissions.Add(new RolePermission()
                {
                    RoleId = roleId,
                    PermissionId = permissionId
                });
            }
            _context.SaveChanges();
        }

        public void AddUserRoles(int userId, List<int> roleIds)
        {
            foreach (var roleId in roleIds)
            {
                _context.UserRoles.Add(new UserRole()
                {
                    UserId = userId,
                    RoleId = roleId
                });
            }
            _context.SaveChanges();
        }

        public void EditRolePermissions(int roleId, List<int> permissionIds)
        {
            _context.RolePermissions.Where(rp => rp.RoleId == roleId).ToList().ForEach(rp => _context.RolePermissions.Remove(rp));
            foreach (var permissionId in permissionIds)
            {
                _context.RolePermissions.Add(new RolePermission()
                {
                    RoleId=roleId,
                    PermissionId=permissionId
                });
            }
            _context.SaveChanges();
        }

        public void EditUserRoles(int userId, List<int> roleIds)
        {
            _context.UserRoles.Where(ur => ur.UserId == userId).ToList().ForEach(ur => _context.UserRoles.Remove(ur));
            foreach (var roleId in roleIds)
            {
                _context.UserRoles.Add(new UserRole()
                {
                    UserId = userId,
                    RoleId = roleId
                });
            }
            _context.SaveChanges();
        }

        public List<Permission> GetAllPermissions()
        {
            return _context.Permissions.ToList();
        }

        public List<Role> GetAllRoles()
        {
            return _context.Roles.ToList();
        }

        public Role GetRoleById(int roleId)
        {
            return _context.Roles.Find(roleId);
        }

        public Role GetRoleByIdNoTracking(int roleId)
        {
            return _context.Roles.AsNoTracking().SingleOrDefault(r => r.RoleId == roleId);
        }

        public List<int> GetRolePermissions(int roleId)
        {
            return _context.RolePermissions.Where(rp => rp.RoleId == roleId).Select(rp => rp.PermissionId).ToList();
        }

        public List<int> GetUserRoles(int userId)
        {
            return _context.UserRoles.Where(ur => ur.UserId == userId).Select(ur=>ur.RoleId).ToList();
        }

        public bool IsExistRole(string roleTitle)
        {
            return _context.Roles.Any(r => r.RoleTitle == roleTitle);
        }

        public void UpdateRole(Role role)
        {
            _context.Roles.Update(role);
            _context.SaveChanges();
        }
    }
}
