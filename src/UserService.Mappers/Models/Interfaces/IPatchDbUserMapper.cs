using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using Microsoft.AspNetCore.JsonPatch;

namespace LT.DigitalOffice.UserService.Mappers.Models.Interfaces
{
    public interface IPatchDbUserMapper
    {
        JsonPatchDocument<DbUser> Map(JsonPatchDocument<EditUserRequest> request);
    }
}