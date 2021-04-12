using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

[assembly: InternalsVisibleTo("LT.DigitalOffice.UserService.Business.UnitTests")]
[assembly: InternalsVisibleTo("LT.DigitalOffice.UserService.Mappers.UnitTests")]
namespace LT.DigitalOffice.UserService.Business.Helpers.Password
{
    internal static class UserPasswordHash
    {
        private const string INTERNAL_SALT = "LT.DigitalOffice.SALT3";

        internal static string GetPasswordHash(string userLogin, string salt, string userPassword)
        {
            return Encoding.UTF8.GetString(new SHA512Managed().ComputeHash(
                    Encoding.UTF8.GetBytes($"{salt}{userLogin}{userPassword}{INTERNAL_SALT}")));
        }
    }
}