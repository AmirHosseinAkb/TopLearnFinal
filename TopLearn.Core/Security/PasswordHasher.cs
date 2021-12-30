using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TopLearn.Core.Security
{
    public class PasswordHasher
    {
        public static string HashPasswordMD5(string password)
        {
            Byte[] originalPass;
            Byte[] encodedPass;
            MD5 md5;
            md5 = new MD5CryptoServiceProvider();
            originalPass = ASCIIEncoding.Default.GetBytes(password);
            encodedPass = md5.ComputeHash(originalPass);
            return BitConverter.ToString(encodedPass);
        }
    }
}
