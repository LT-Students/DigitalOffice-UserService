using LT.DigitalOffice.Kernel.Exceptions;
using LT.DigitalOffice.UserService.Mappers.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using LT.DigitalOffice.UserService.UserCredentials;
using System;

namespace LT.DigitalOffice.UserService.Mappers
{
    /// <summary>
    /// Represents mapper. Provides methods for converting an object of user value <see cref="UserRequest"/>
    /// type into an object of <see cref="DbUserCredentials"/> type according to some rule.
    /// </summary>
    public class UserCredentialsMapper : IMapper<UserRequest, DbUserCredentials>
    {
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
                PasswordHash = UserPassword.GetPasswordHash(value.Login, salt, value.Password)
            };
        }
    }
}
