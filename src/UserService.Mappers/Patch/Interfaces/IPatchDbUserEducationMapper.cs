using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Education;
using Microsoft.AspNetCore.JsonPatch;

namespace LT.DigitalOffice.UserService.Mappers.Patch.Interfaces
{
    [AutoInject]
    public interface IPatchDbUserEducationMapper
    {
        JsonPatchDocument<DbUserEducation> Map(JsonPatchDocument<EditEducationRequest> request);
    }
}
