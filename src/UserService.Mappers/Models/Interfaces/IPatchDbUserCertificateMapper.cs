using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Certificates;
using Microsoft.AspNetCore.JsonPatch;
using System;

namespace LT.DigitalOffice.UserService.Mappers.Models.Interfaces
{
    [AutoInject]
    public interface IPatchDbUserCertificateMapper
    {
        JsonPatchDocument<DbUserCertificate> Map(JsonPatchDocument<EditCertificateRequest> request, Guid? imageId);
    }
}
