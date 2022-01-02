using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopLearn.Core.DTOs.User;
using TopLearn.Data.Entities.User;
using TopLearn.Data.Entities.Wallet;

namespace TopLearn.Core.Services.Interfaces
{
    public interface IUserService
    {
        #region Account
        bool IsExistUserByEmail(string email);
        bool IsExistUserByUserName(string userName);
        void AddUser(User user);
        bool ActiveAccount(string activeCode);
        void UpdateUser(User user);
        User GetUserForLogin(string email, string password);
        User GetUserByEmail(string email);
        User GetUserByUserName(string userName);
        int GetUserIdByUserName(string userName);
        User GetUserByActiveCode(string activeCode);
        #endregion

        #region UserPanel

        UserInformationsForShowViewModel GetUserInformationsForShow(string userName);
        SideBarInformationsViewModel GetUserSideBarInformationsForShow(string userName);
        EditUserProfileViewModel GetUserForEdit(string userName);
        void EditUser(string userName, EditUserProfileViewModel edit);

        #endregion

        #region Wallet

        int AddWallet(Wallet wallet);
        Wallet GetWalletByWalletId(int walletId);
        void UpdateWallet(Wallet wallet);
        List<Wallet> GetUserWallets(string userName);
        int BalanceUserWallet(string userName);

        #endregion

        #region AdminUsers

        UsersForShowInAdminViewModel GetUsers(int pageId = 1, string filterUserName = "", string filterEmail = "");
        int AddUserFromAdmin(CreateUserViewModel create);

        #endregion
    }
}
