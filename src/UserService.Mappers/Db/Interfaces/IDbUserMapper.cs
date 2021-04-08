using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using System;

namespace LT.DigitalOffice.UserService.Mappers.Db.Interfaces
{
    /// <summary>
    /// Represents mapper. Provides methods for converting an object of <see cref="CreateUserRequest"/>
    /// type into an object of <see cref="DbUser"/> type according to some rule.
    /// </summary>
    [AutoInject]
    public interface IDbUserMapper
    {
        DbUser Map(CreateUserRequest request, Guid? avatarImageId);
    }
}
