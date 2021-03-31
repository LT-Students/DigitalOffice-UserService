using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using System;

namespace LT.DigitalOffice.UserService.Mappers.DbMappers.Interfaces
{
    /// <summary>
    /// Represents mapper. Provides methods for converting an object of user value <see cref="CreateUserRequest"/>
    /// type into an object of <see cref="DbUserCredentials"/> type according to some rule.
    /// </summary>
    public interface IDbUserCredentialsMapper
    {
        DbUserCredentials Map(CreateUserRequest request);
    }
}
