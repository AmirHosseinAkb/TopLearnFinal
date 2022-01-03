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
using TopLearn.Data.Entities.Wallet;

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

        public int AddUserFromAdmin(CreateUserViewModel create)
        {
            User user = new User()
            {
                UserName = create.UserName,
                Email = EmailConvertor.FixEmail(create.Email),
                Password = PasswordHasher.HashPasswordMD5(create.Password),
                RegisterDate = DateTime.Now,
                ActiveCode = NameGenerator.GenerateUniqCode(),
                IsActive = true,
                AvatarName = "Default.png",
                IsDeleted=false
            };
            if (create.UserAvatar != null)
            {
                user.AvatarName = NameGenerator.GenerateUniqCode() + Path.GetExtension(create.UserAvatar.FileName);
                string imagePath = Path.Combine(Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "UserAvatar",
                    user.AvatarName);
                
                using(var stream=new FileStream(imagePath, FileMode.Create))
                {
                    create.UserAvatar.CopyTo(stream);
                }
            }
            UpdateUser(user);
            return user.UserId;
        }

        public int AddWallet(Wallet wallet)
        {
            _context.Wallets.Add(wallet);
            _context.SaveChanges();
            return wallet.WalletId;
        }

        public int BalanceUserWallet(string userName)
        {
            var userId = GetUserIdByUserName(userName);
            var deposite = _context.Wallets.Where(w => w.UserId == userId && w.TypeId == 1 && w.IsPayed).ToList();
            var withdraw = _context.Wallets.Where(w => w.UserId == userId && w.TypeId == 2 && w.IsPayed).ToList();
            return deposite.Sum(w => w.Amount) - withdraw.Sum(w => w.Amount);

        }

        public void EditUser(string userName, EditUserProfileViewModel edit)
        {
            var user = GetUserByUserName(userName);
            user.Email = EmailConvertor.FixEmail(edit.Email);
            if (edit.UserAvatar != null)
            {
                string imagePath = "";
                if (edit.AvatarName != "Default.png")
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

        public void EditUserFromAdmin(EditUserViewModel edit)
        {
            var user = GetUserById(edit.UserId);

            user.Email = EmailConvertor.FixEmail(edit.Email);
            if (!string.IsNullOrEmpty(edit.Password))
            {
                user.Password = PasswordHasher.HashPasswordMD5(edit.Password);
            }
            if (edit.UserAvatar != null)
            {
                string imagePath = "";
                if (edit.AvatarName != "Default.png")
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
                using(var stream=new FileStream(imagePath, FileMode.Create))
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

        public User GetUserById(int userId)
        {
            return _context.Users.Find(userId);
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

        public EditUserViewModel GetUserForEditFromAdmin(int userId)
        {
            return _context.Users.Where(u => u.UserId == userId)
                .Select(u => new EditUserViewModel()
                {
                    UserId = u.UserId,
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
                    Wallet = BalanceUserWallet(userName)
                }).Single();
        }

        public UsersForShowInAdminViewModel GetUsers(int pageId = 1, string filterUserName = "", string filterEmail = "")
        {
            IQueryable<User> result = _context.Users;
            if (!string.IsNullOrEmpty(filterUserName))
            {
                result = result.Where(u => u.UserName.Contains(filterUserName));
            }

            if (!string.IsNullOrEmpty(filterEmail))
            {
                result = result.Where(u => u.Email.Contains(filterEmail));
            }

            int take = 1;
            int skip = (pageId - 1) * take;
            int pageCount= result.Count() / take;

            if (result.Count() % take != 0)
            {
                pageCount += 1;
            }

            UsersForShowInAdminViewModel users = new UsersForShowInAdminViewModel()
            {
                Users = result.Skip(skip).Take(take).ToList(),
                CurrentPage = pageId,
                PageCount = pageCount
            };
            
            return users;
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

        public List<Wallet> GetUserWallets(string userName)
        {
            var userId = GetUserIdByUserName(userName);
            return _context.Wallets.Where(w => w.UserId == userId).ToList();
        }

        public Wallet GetWalletByWalletId(int walletId)
        {
            return _context.Wallets.Find(walletId);
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

        public void UpdateWallet(Wallet wallet)
        {
            _context.Wallets.Update(wallet);
            _context.SaveChanges();
        }
    }
}
