using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using System;
using System.Linq;

namespace LT.DigitalOffice.UserService.Mappers.Models
{
    public class PatchDbUserMapper : IPatchDbUserMapper
    {
        public JsonPatchDocument<DbUser> Map(
            JsonPatchDocument<EditUserRequest> request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var result = new JsonPatchDocument<DbUser>();

            Func<Operation<EditUserRequest>, string> value = item => !string.IsNullOrEmpty(item.value?.ToString().Trim())
                ? item.value.ToString().Trim() : null;

            foreach (Operation<EditUserRequest> item in request.Operations)
            {
                if (item.path.EndsWith(nameof(EditUserRequest.RoleId), StringComparison.OrdinalIgnoreCase)
                    || item.path.EndsWith(nameof(EditUserRequest.OfficeId), StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (item.path.EndsWith(nameof(EditUserRequest.Status), StringComparison.OrdinalIgnoreCase))
                {
                    result.Operations.Add(new Operation<DbUser>(item.op, item.path, item.from, (int)Enum.Parse(typeof(UserStatus), item.value.ToString())));
                    continue;
                }
                if (item.path.EndsWith(nameof(EditUserRequest.Gender), StringComparison.OrdinalIgnoreCase))
                {
                    result.Operations.Add(new Operation<DbUser>(item.op, item.path, item.from, (int)Enum.Parse(typeof(UserGender), item.value.ToString())));
                    continue;
                }
                if (item.path.EndsWith(nameof(EditUserRequest.DateOfBirth), StringComparison.OrdinalIgnoreCase))
                {
                    result.Operations.Add(new Operation<DbUser>(item.op, item.path, item.from, DateTime.TryParse(value(item), out DateTime date) ? date: null));
                    continue;
                }
                if (item.path.EndsWith(nameof(EditUserRequest.StartWorkingAt), StringComparison.OrdinalIgnoreCase))
                {
                    result.Operations.Add(new Operation<DbUser>(item.op, item.path, item.from, DateTime.TryParse(value(item), out DateTime date) ? date : null));
                    continue;
                }
                if (item.path.EndsWith(nameof(EditUserRequest.City), StringComparison.OrdinalIgnoreCase))
                {
                    result.Operations.Add(new Operation<DbUser>(item.op, item.path, item.from, value(item)));
                    continue;
                }

                result.Operations.Add(new Operation<DbUser>(item.op, item.path, item.from, item.value));
            }

            return result;
        }
    }
}