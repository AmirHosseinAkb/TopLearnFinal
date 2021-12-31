using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopLearn.Core.Services.Interfaces;
using TopLearn.Data.Context;
using TopLearn.Core.Convertors;
using TopLearn.Core.Generators;
using TopLearn.Core.Security;
using TopLearn.Data.Entities.User;
using TopLearn.Core.DTOs.User;

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

        public void EditUser(string userName, EditUserProfileViewModel edit)
        {
            var user = GetUserByUserName(userName);
            user.Email = EmailConvertor.FixEmail(edit.Email);
            if (edit.UserAvatar != null)
            {
                string imagePath = "";
                if (edit.AvatarName == "Default.png")
                {
                    imagePath = Path.Combine(Directory.GetCurrentDirectory(),
                        "wwwroot",
                        "UserAvatar",
                        edit.AvatarName);
                    if (File.Exists(imagePath))
                    {
                        File.Delete(imagePath);
                    }
                }

                user.AvatarName = NameGenerator.GenerateUniqCode() + Path.GetExtension(edit.UserAvatar.FileName);
                imagePath = Path.Combine(Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "UserAvatar",
                    user.AvatarName);
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    edit.UserAvatar.CopyTo(stream);
                }
            }
            UpdateUser(user);
        }

        public User GetUserByActiveCode(string activeCode)
        {
            return _context.Users.SingleOrDefault(u => u.ActiveCode == activeCode);
        }

        public User GetUserByEmail(string email)
        {
            return _context.Users.SingleOrDefault(e => e.Email == EmailConvertor.FixEmail(email));
        }

        public User GetUserByUserName(string userName)
        {
            return _context.Users.SingleOrDefault(u => u.UserName == userName);
        }

        public EditUserProfileViewModel GetUserForEdit(string userName)
        {
            return _context.Users.Where(u => u.UserName == userName)
                .Select(u => new EditUserProfileViewModel()
                {
                    UserName = u.UserName,
                    Email = u.Email,
                    AvatarName = u.AvatarName
                }).Single();
        }

        public User GetUserForLogin(string email, string password)
        {
            return _context.Users.SingleOrDefault(u =>
                u.Email == EmailConvertor.FixEmail(email) && u.Password== PasswordHasher.HashPasswordMD5(password));
        }

        public int GetUserIdByUserName(string userName)
        {
            return _context.Users.SingleOrDefault(u => u.UserName == userName).UserId;
        }

        public UserInformationsForShowViewModel GetUserInformationsForShow(string userName)
        {
            return _context.Users.Where(u => u.UserName == userName)
                .Select(u => new UserInformationsForShowViewModel()
                {
                    UserName = u.UserName,
                    Email = u.Email,
                    RegisterDate = u.RegisterDate,
                    Wallet = 0
                }).Single();
        }

        public SideBarInformationsViewModel GetUserSideBarInformationsForShow(string userName)
        {
            return _context.Users.Where(u => u.UserName == userName)
                .Select(u=>new SideBarInformationsViewModel()
                {
                    UserName=u.UserName,
                    AvatarName = u.AvatarName,
                    RegisterDate = u.RegisterDate
                }).Single();
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
