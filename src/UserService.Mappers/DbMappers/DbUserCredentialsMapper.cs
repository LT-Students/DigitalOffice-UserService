using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.UserService.Mappers.DbMappers.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using System;

namespace LT.DigitalOffice.UserService.Mappers.DbMappers
{
    public class DbUserCredentialsMapper : IDbUserCredentialsMapper
    {
        public DbUserCredentials Map(CreateUserRequest request)
        {

            var salt = $"{ Guid.NewGuid() }{ Guid.NewGuid() }";

            return new DbUserCredentials
            {
                Id = Guid.NewGuid(),
                Login = request.Login,
                Salt = salt
            };
        }
    }
}
