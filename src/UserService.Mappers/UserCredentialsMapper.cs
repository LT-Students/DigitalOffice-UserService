using LT.DigitalOffice.UserService.Mappers.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using System;
using System.Security.Cryptography;
using System.Text;

namespace LT.DigitalOffice.UserService.Mappers
{
    /// <summary>
    /// Represents mapper. Provides methods for converting an object of user request <see cref="UserRequest"/>
    /// type into an object of <see cref="DbUserCredentials"/> type according to some rule.
    /// </summary>
    public class UserCredentialsMapper : IMapper<UserRequest, DbUserCredentials>
    {
        public DbUserCredentials Map(UserRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return new DbUserCredentials
            {
                Login = request.Login,
                PasswordHash = Encoding.UTF8.GetString(new SHA512Managed().ComputeHash(
                    Encoding.UTF8.GetBytes(request.Password)))
            };
        }
    }
}
