using LT.DigitalOffice.UserService.Mappers.Patch.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Communication;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using System;

namespace LT.DigitalOffice.UserService.Mappers.Patch
{
  public class PatchDbUserCommunicationMapper : IPatchDbUserCommunicationMapper
  {
    public JsonPatchDocument<DbUserCommunication> Map(JsonPatchDocument<EditCommunicationRequest> request)
    {
      if (request is null)
      {
        return default;
      }

      JsonPatchDocument<DbUserCommunication> result = new();

      foreach (Operation item in request.Operations)
      {
        if (item.path.EndsWith(nameof(EditCommunicationRequest.Type), StringComparison.OrdinalIgnoreCase))
        {
          result.Operations.Add(new Operation<DbUserCommunication>(
            item.op,
            item.path,
            item.from,
            (int)Enum.Parse(typeof(CommunicationType), item.value.ToString())));

          continue;
        }
        else if (item.path.EndsWith(nameof(EditCommunicationRequest.Value), StringComparison.OrdinalIgnoreCase))
        {
          result.Operations.Add(new Operation<DbUserCommunication>(
            "replace",
            nameof(DbUserCommunication.IsConfirmed),
            null,
            false));
        }

        result.Operations.Add(new Operation<DbUserCommunication>(
          item.op,
          item.path,
          item.from,
          item.value));
      }

      return result;
    }
  }
}
