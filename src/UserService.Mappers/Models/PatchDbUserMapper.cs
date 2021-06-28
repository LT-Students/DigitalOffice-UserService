using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using System;

namespace LT.DigitalOffice.UserService.Mappers.Models
{
    public class PatchDbUserMapper : IPatchDbUserMapper
    {
        public JsonPatchDocument<DbUser> Map(
            JsonPatchDocument<EditUserRequest> request,
            Guid? imageId)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var result = new JsonPatchDocument<DbUser>();

            foreach (var item in request.Operations)
            {
                if (item.path.EndsWith(nameof(EditUserRequest.Status), StringComparison.OrdinalIgnoreCase))
                {
                    result.Operations.Add(new Operation<DbUser>(item.op, item.path, item.from, Enum.Parse(typeof(UserStatus), item.value.ToString())));
                    continue;
                }
                if (item.path.EndsWith(nameof(EditUserRequest.Gender), StringComparison.OrdinalIgnoreCase))
                {
                    result.Operations.Add(new Operation<DbUser>(item.op, item.path, item.from, Enum.Parse(typeof(UserGender), item.value.ToString())));
                    continue;
                }
                if (item.path.EndsWith(nameof(EditUserRequest.AvatarImage), StringComparison.OrdinalIgnoreCase))
                {
                    result.Operations.Add(new Operation<DbUser>(item.op, $"/{nameof(DbUser.AvatarFileId)}", item.from, imageId));
                    continue;
                }
                if (item.path.EndsWith(nameof(EditUserRequest.DateOfBirth), StringComparison.OrdinalIgnoreCase))
                {
                    result.Operations.Add(new Operation<DbUser>(item.op, $"/{nameof(DbUser.DateOfBirth)}", item.from, DateTime.Parse(item.value.ToString())));
                    continue;
                }
                if (item.path.EndsWith(nameof(EditUserRequest.StartWorkingAt), StringComparison.OrdinalIgnoreCase))
                {
                    result.Operations.Add(new Operation<DbUser>(item.op, $"/{nameof(DbUser.StartWorkingAt)}", item.from, DateTime.Parse(item.value.ToString())));
                    continue;
                }
                    result.Operations.Add(new Operation<DbUser>(item.op, item.path, item.from, item.value));
            }

            return result;
        }
    }
}