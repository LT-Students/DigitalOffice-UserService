using System;
using System.Runtime.CompilerServices;
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

        private const string PASSWORD = "admin";
        private const string INTERNAL_SALT = "LT.DigitalOffice.SALT3";

        internal static string salt = $"{Guid.NewGuid()}{Guid.NewGuid()}";

        internal static string GetPasswordHash()
        {
            salt = $"{Guid.NewGuid()}{Guid.NewGuid()}";

            return GetAdminPasswordHash();
        }

        private static string GetAdminPasswordHash()
        {
            return Encoding.UTF8.GetString(new SHA512Managed().ComputeHash(
                    Encoding.UTF8.GetBytes($"{salt}{LOGIN}{PASSWORD}{INTERNAL_SALT}")));
        }
    }
}
