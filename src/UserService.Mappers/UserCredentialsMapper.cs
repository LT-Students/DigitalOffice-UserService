using LT.DigitalOffice.UserService.Mappers.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using System;
using System.Security.Cryptography;
using System.Text;

namespace LT.DigitalOffice.UserService.Mappers
{
    /// <summary>
    /// Represents mapper. Provides methods for converting an object of user request
    /// type into an object of <see cref="DbUserCredentials"/> type according to some rule.
    /// </summary>
    public class UserCredentialsMapper : IMapper<EditUserRequest, string, DbUserCredentials>,
        IMapper<UserCreateRequest, Guid, DbUserCredentials>
    {
        public DbUserCredentials Map(UserCreateRequest request, Guid userId)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return new DbUserCredentials
            {
                UserId = userId,
                Email = request.Email,
                PasswordHash = Encoding.UTF8.GetString(new SHA512Managed().ComputeHash(
                    Encoding.UTF8.GetBytes(request.Password))),
                Salt = "Example_salt"
            };
        }

        public DbUserCredentials Map(EditUserRequest request, string salt)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return new DbUserCredentials
            {
                UserId = request.Id,
                Email = request.Email,
                PasswordHash = Encoding.UTF8.GetString(new SHA512Managed().ComputeHash(
                    Encoding.UTF8.GetBytes(request.Password))),
                Salt = salt
            };
        }
    }
}
