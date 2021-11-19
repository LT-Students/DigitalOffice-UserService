using LT.DigitalOffice.UserService.Mappers.Patch.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using System;

namespace LT.DigitalOffice.UserService.Patch.Models
{
  public class PatchDbUserMapper : IPatchDbUserMapper
  {
    public (JsonPatchDocument<DbUser> dbUserPatch, JsonPatchDocument<DbUserLocation> dbUserLocationPatch) Map(
      JsonPatchDocument<EditUserRequest> request)
    {
      if (request is null)
      {
        return (null, null);
      }

      JsonPatchDocument<DbUser> dbUserPatch = new();
      JsonPatchDocument<DbUserLocation> dbUserLocationPatch = new();

      Func<Operation<EditUserRequest>, string> value = item => !string.IsNullOrEmpty(item.value?.ToString().Trim())
        ? item.value.ToString().Trim() : null;

      foreach (Operation<EditUserRequest> item in request.Operations)
      {
        if (item.path.EndsWith(nameof(EditUserRequest.Gender), StringComparison.OrdinalIgnoreCase))
        {
          dbUserPatch.Operations.Add(new Operation<DbUser>(item.op, item.path, item.from, (int)Enum.Parse(typeof(UserGender), item.value.ToString())));
          continue;
        }

        if (item.path.EndsWith(nameof(EditUserRequest.Status), StringComparison.OrdinalIgnoreCase))
        {
          dbUserPatch.Operations.Add(new Operation<DbUser>(item.op, item.path, item.from, (int)Enum.Parse(typeof(UserStatus), item.value.ToString())));
          continue;
        }

        if (item.path.EndsWith(nameof(EditUserRequest.DateOfBirth), StringComparison.OrdinalIgnoreCase)
          || item.path.EndsWith(nameof(EditUserRequest.StartWorkingAt), StringComparison.OrdinalIgnoreCase))
        {
          dbUserPatch.Operations.Add(new Operation<DbUser>(item.op, item.path, item.from, DateTime.TryParse(value(item), out DateTime date) ? date : null));
          continue;
        }

        if (item.path.EndsWith(nameof(EditUserRequest.Latitude), StringComparison.OrdinalIgnoreCase)
          || item.path.EndsWith(nameof(EditUserRequest.Longitude), StringComparison.OrdinalIgnoreCase))
        {
          dbUserLocationPatch.Operations.Add(new Operation<DbUserLocation>(item.op, item.path, item.from, double.TryParse(value(item), out double v) ? v : null));
          continue;
        }

        if (item.path.EndsWith(nameof(EditUserRequest.BusinessHoursFromUtc), StringComparison.OrdinalIgnoreCase)
          || item.path.EndsWith(nameof(EditUserRequest.BusinessHoursToUtc), StringComparison.OrdinalIgnoreCase))
        {
          dbUserLocationPatch.Operations.Add(new Operation<DbUserLocation>(item.op, item.path, item.from, DateTime.TryParse(value(item), out DateTime date) ? date : null));
          continue;
        }

        dbUserPatch.Operations.Add(new Operation<DbUser>(item.op, item.path, item.from, item.value));
      }

      return (dbUserPatch, dbUserLocationPatch);
    }
  }
}