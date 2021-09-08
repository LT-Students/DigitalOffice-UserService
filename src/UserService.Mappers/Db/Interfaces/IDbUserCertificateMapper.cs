using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Certificates;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Mappers.Db.Interfaces
{
  [AutoInject]
    public interface IDbUserCertificateMapper
    {
        DbUserCertificate Map(CreateCertificateRequest request, List<Guid> imagesIds);
    }
}
