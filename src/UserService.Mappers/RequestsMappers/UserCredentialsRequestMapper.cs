using LT.DigitalOffice.Kernel.Exceptions;
using LT.DigitalOffice.UserService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using LT.DigitalOffice.UserService.UserCredentials;
using System;

namespace LT.DigitalOffice.UserService.Mappers.RequestsMappers
{
    public class UserCredentialsRequestMapper : IUserCredentialsRequestMapper
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
