using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Models;

namespace LT.DigitalOffice.UserService.Mappers.Models.Interfaces
{
    public interface ICertificateInfoMapper
    {
        CertificateInfo Map(
            DbUserCertificate dbUserCertificate,
            ImageInfo image);
    }
}
