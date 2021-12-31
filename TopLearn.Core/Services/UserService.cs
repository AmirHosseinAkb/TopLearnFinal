using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopLearn.Core.Services.Interfaces;
using TopLearn.Data.Context;
using TopLearn.Core.Convertors;
using TopLearn.Core.Generators;
using TopLearn.Core.Security;
using TopLearn.Data.Entities.User;

namespace TopLearn.Core.Services
{
    public class UserService : IUserService
    {
        private TopLearnContext _context;
        public UserService(TopLearnContext context)
        {
            _context = context;
        }

        public bool ActiveAccount(string activeCode)
        {
            var user = _context.Users.SingleOrDefault(u => u.ActiveCode == activeCode);
            if (user == null || user.IsActive)
            {
                return false;
            }

            user.IsActive = true;
            user.ActiveCode = NameGenerator.GenerateUniqCode();
            UpdateUser(user);
            return true;
        }

        public void AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public User GetUserByActiveCode(string activeCode)
        {
            return _context.Users.SingleOrDefault(u => u.ActiveCode == activeCode);
        }

        public User GetUserByEmail(string email)
        {
            return _context.Users.SingleOrDefault(e => e.Email == EmailConvertor.FixEmail(email));
        }

        public User GetUserForLogin(string email, string password)
        {
            return _context.Users.SingleOrDefault(u =>
                u.Email == EmailConvertor.FixEmail(email) && u.Password== PasswordHasher.HashPasswordMD5(password));
        }

        public bool IsExistUserByEmail(string email)
        {
            return _context.Users.Any(u => u.Email == EmailConvertor.FixEmail(email)) ;
        }

        public bool IsExistUserByUserName(string userName)
        {
            return _context.Users.Any(u => u.UserName == userName);
        }

        public void UpdateUser(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }
    }
}
