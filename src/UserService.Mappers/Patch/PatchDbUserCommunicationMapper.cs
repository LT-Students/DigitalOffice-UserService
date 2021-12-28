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
      if (request == null)
      {
        throw new ArgumentNullException(nameof(request));
      }

      var result = new JsonPatchDocument<DbUserCommunication>();

      foreach (var item in request.Operations)
      {
        if (item.path.EndsWith(nameof(EditCommunicationRequest.Type), StringComparison.OrdinalIgnoreCase))
        {
          result.Operations.Add(new Operation<DbUserCommunication>(
              item.op, item.path, item.from, (int)Enum.Parse(typeof(CommunicationType), item.value.ToString())));
          continue;
        }

        result.Operations.Add(new Operation<DbUserCommunication>(item.op, item.path, item.from, item.value));
      }

      return result;
    }
  }
}
