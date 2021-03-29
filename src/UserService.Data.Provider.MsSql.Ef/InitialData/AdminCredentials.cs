using System;
using System.Security.Cryptography;
using System.Text;

namespace LT.DigitalOffice.UserService.UserCredentials.Admin
{
    internal static class AdminCredentials
    {
        internal const string LOGIN = "admin";
        internal const string EMAIL = "admin@lanit-tercom.com";
        internal const string FIRST_NAME = "admin@lt";
        internal const string LAST_NAME = "admin@lt";

        internal static string salt;
        internal static Guid userId = new Guid("6146B87A-587D-4945-A565-1CBDE93F187C");

        private static string _defaultPassword = "admin";
        private const string INTERNAL_SALT = "LT.DigitalOffice.SALT3";

        internal static string GetPasswordHash(string newPassword = null)
        {
            string password = newPassword ?? _defaultPassword;

            salt = $"{Guid.NewGuid()}{Guid.NewGuid()}";

            return GetAdminPasswordHash(password);
        }

        private static string GetAdminPasswordHash(string password)
        {
            return Encoding.UTF8.GetString(new SHA512Managed().ComputeHash(
                    Encoding.UTF8.GetBytes($"{salt}{LOGIN}{password}{INTERNAL_SALT}")));
        }
    }
}
