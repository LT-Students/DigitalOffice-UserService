using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Mappers.Models.Interfaces
{
    [AutoInject]
    public interface ICertificateInfoMapper
    {
        CertificateInfo Map(
            DbUserCertificate dbUserCertificate,
            List<ImageInfo> images);
    }
}
