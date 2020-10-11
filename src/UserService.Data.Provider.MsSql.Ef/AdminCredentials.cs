using System;
using System.Security.Cryptography;
using System.Text;

namespace LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef
{
    static class AdminCredentials
    {
        public static string salt = Guid.NewGuid().ToString();
        private static string password = "admin";

        public static string GetPasswordHash()
        {
            return Encoding.UTF8.GetString(new SHA512Managed().ComputeHash(
                    Encoding.UTF8.GetBytes($"{password}_{salt}")));
        }
    }
}
