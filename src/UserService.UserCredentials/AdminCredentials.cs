using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef")]

namespace LT.DigitalOffice.UserService.UserCredentials.Admin
{
    internal static class AdminCredentials
    {
        private const string PASSWORD = "admin";

        internal const string LOGIN = "admin";
        internal const string EMAIL = "admin@lanit-tercom.com";
        internal const string FIRST_NAME = "admin@lt";
        internal const string LAST_NAME = "admin@lt";

        internal static string salt = $"{Guid.NewGuid()}{Guid.NewGuid()}";

        internal static string GetPasswordHash()
        {
            return UserPassword.GetPasswordHash(LOGIN, salt, PASSWORD);
        }
    }
}
