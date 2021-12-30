using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopLearn.Data.Entities.User;

namespace TopLearn.Core.Services.Interfaces
{
    public interface IUserService
    {
        bool IsExistUserByEmail(string email);
        bool IsExistUserByUserName(string userName);
        void AddUser(User user);
        bool ActiveAccount(string activeCode);
        void UpdateUser(User user);
    }
}
