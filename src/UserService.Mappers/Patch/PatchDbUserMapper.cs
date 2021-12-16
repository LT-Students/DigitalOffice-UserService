﻿using LT.DigitalOffice.UserService.Mappers.Patch.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using System;

namespace LT.DigitalOffice.UserService.Mappers.Patch
{
  public class PatchDbUserMapper : IPatchDbUserMapper
  {
    public JsonPatchDocument<DbUser> Map(
      JsonPatchDocument<EditUserRequest> request)
    {
      if (request is null)
      {
        return null;
      }

      var result = new JsonPatchDocument<DbUser>();

      Func<Operation<EditUserRequest>, string> value = item => !string.IsNullOrEmpty(item.value?.ToString().Trim())
          ? item.value.ToString().Trim() : null;

      foreach (Operation<EditUserRequest> item in request.Operations)
      {
        if (item.path.EndsWith(nameof(EditUserRequest.Status), StringComparison.OrdinalIgnoreCase))
        {
          result.Operations.Add(new Operation<DbUser>(item.op, item.path, item.from, (int)Enum.Parse(typeof(UserStatus), item.value.ToString())));
          continue;
        }

        result.Operations.Add(new Operation<DbUser>(item.op, item.path, item.from, item.value));
      }

      return result;
    }
  }
}