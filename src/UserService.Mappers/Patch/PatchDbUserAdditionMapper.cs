using LT.DigitalOffice.UserService.Mappers.Patch.interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Mappers.Patch
{
  public class PatchDbUserAdditionMapper : IPatchDbUserAdditionMapper
  {
    public JsonPatchDocument<DbUserAddition> Map(
     JsonPatchDocument<EditUserRequest> request)
    {
      if (request is null)
      {
        return null;
      }

      var result = new JsonPatchDocument<DbUserAddition>();

      Func<Operation<EditUserRequest>, string> value = item => !string.IsNullOrEmpty(item.value?.ToString().Trim())
       ? item.value.ToString() : null;

      foreach (Operation<EditUserRequest> item in request.Operations)
      {
        if (item.path.EndsWith(nameof(EditUserRequest.DateOfBirth), StringComparison.OrdinalIgnoreCase))
        {
          result.Operations.Add(new Operation<DbUserAddition>(item.op, item.path, item.from, DateTime.TryParse(value(item), out DateTime date) ? date : null));
          continue;
        }

        if (item.path.EndsWith(nameof(EditUserRequest.BusinessHoursFromUtc), StringComparison.OrdinalIgnoreCase))
        {
          result.Operations.Add(new Operation<DbUserAddition>(item.op, item.path, item.from, DateTime.TryParse(value(item), out DateTime date) ? date : null));
          continue;
        }

        if (item.path.EndsWith(nameof(EditUserRequest.BusinessHoursToUtc), StringComparison.OrdinalIgnoreCase))
        {
          result.Operations.Add(new Operation<DbUserAddition>(item.op, item.path, item.from, DateTime.TryParse(value(item), out DateTime date) ? date : null));
          continue;
        }

        result.Operations.Add(new Operation<DbUserAddition>(item.op, item.path, item.from, item.value));
      }

      return result;
    }
  }
}
