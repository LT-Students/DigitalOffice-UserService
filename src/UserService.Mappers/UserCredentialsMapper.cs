using LT.DigitalOffice.Kernel.Exceptions;
using LT.DigitalOffice.UserService.Mappers.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using System;
using System.Security.Cryptography;
using System.Text;

namespace LT.DigitalOffice.UserService.Mappers
{
    /// <summary>
    /// Represents mapper. Provides methods for converting an object of user value <see cref="UserRequest"/>
    /// type into an object of <see cref="DbUserCredentials"/> type according to some rule.
    /// </summary>
    public class UserCredentialsMapper : IMapper<UserRequest, DbUserCredentials>
    {
        internal static string SALT3 = "LT.DigitalOffice.SALT3";

        public DbUserCredentials Map(UserRequest value)
        {
            if (value?.Id == null || value.Id == Guid.Empty)
            {
                throw new BadRequestException();
            }

            var salt = $"{ Guid.NewGuid() }{ Guid.NewGuid() }";

            return new DbUserCredentials
            {
                Id = Guid.NewGuid(),
                UserId = value.Id.Value,
                Login = value.Login,
                Salt = salt,
                PasswordHash = Encoding.UTF8.GetString(new SHA512Managed().ComputeHash(
                    Encoding.UTF8.GetBytes($"{ salt }{ request.Login }{ request.Password }{ SALT3 }")))
            };
        }
    }
}
