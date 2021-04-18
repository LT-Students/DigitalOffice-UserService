using System;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using Microsoft.AspNetCore.JsonPatch;

namespace LT.DigitalOffice.UserService.Mappers.Models.Interfaces
{
    [AutoInject]
    public interface IPatchDbUserMapper
    {
        JsonPatchDocument<DbUser> Map(
            JsonPatchDocument<EditUserRequest> request,
            Func<string, Guid?> getAvatarImageId,
            Guid userId);
    }
}