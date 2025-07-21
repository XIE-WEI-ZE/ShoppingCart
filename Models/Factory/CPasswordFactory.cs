using System;
using System.Security.Cryptography;
using System.Text;

namespace prjECommerceDemo.Models.Factory
{
    public class CPasswordFactory
    {
        /// <summary>
        /// 產生 8 位 salt
        /// </summary>
        public static string GetSalt()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 8);
        }

        /// <summary>
        /// 使用 SHA256 加密密碼與 salt 組合
        /// </summary>
        public static string HashPassword(string password, string salt)
        {
            using (SHA256 sha = SHA256.Create())
            {
                string combined = password + salt;
                byte[] bytes = Encoding.UTF8.GetBytes(combined);
                byte[] hash = sha.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
