using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
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
                if (item.path.ToUpper().EndsWith(nameof(EditEducationRequest.FormEducation).ToUpper()))
                {
                    if (Enum.TryParse(item.value.ToString(), out FormEducation education))
                    {
                        dbUserEducation.Operations.Add(new Operation<DbUserEducation>(
                            item.op, $"/{nameof(EditEducationRequest.FormEducation)}", item.from, (int)education));
                        continue;
                    }
                }
                dbUserEducation.Operations.Add(new Operation<DbUserEducation>(item.op, item.path, item.from, item.value));
            }

            return dbUserEducation;
        }
    }
}
