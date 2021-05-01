﻿using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Education;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using System;

namespace LT.DigitalOffice.UserService.Mappers.Models
{
    public class PatchDbUserEducationMapper : IPatchDbUserEducationMapper
    {
        public JsonPatchDocument<DbUserEducation> Map(JsonPatchDocument<EditEducationRequest> request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("Invalid request value");
            }

            JsonPatchDocument<DbUserEducation> dbUserEducation = new();

            foreach (var item in request.Operations)
            {
                dbUserEducation.Operations.Add(new Operation<DbUserEducation>(item.op, item.path, item.from, item.value));
            }

            return dbUserEducation;
        }
    }
}
