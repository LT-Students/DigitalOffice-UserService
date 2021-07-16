using System;
using System.Security.Cryptography;
using System.Text;

namespace LT.DigitalOffice.UserService.UserCredentials.Admin
{
    internal static class AdminCredentials
    {
        private const string InternalSalt = "LT.DigitalOffice.SALT3";
        private const string _defaultPassword = "%4fgT1_3ioR";

        internal const string Login = "admin";
        internal const string Email = "spartak.ryabtsev@lanit-tercom.com";
        internal const string FirstName = "Lanit";
        internal const string LastName = "Tercom";

        internal static string Salt;
        internal static Guid UserId = new Guid("6146B87A-587D-4945-A565-1CBDE93F187C");

        internal static string GetPasswordHash(string newPassword = null)
        {
            string password = newPassword ?? _defaultPassword;

            Salt = $"{Guid.NewGuid()}{Guid.NewGuid()}";

            return GetAdminPasswordHash(password);
        }

        private static string GetAdminPasswordHash(string password)
        {
            return Encoding.UTF8.GetString(new SHA512Managed().ComputeHash(
                    Encoding.UTF8.GetBytes($"{Salt}{Login}{password}{InternalSalt}")));
        }
    }
}
