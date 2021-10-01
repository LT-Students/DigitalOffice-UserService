using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using System;

namespace LT.DigitalOffice.UserService.Mappers.Models
{
    public class CertificateInfoMapper : ICertificateInfoMapper
    {
        public CertificateInfo Map(
            DbUserCertificate dbUserCertificate,
            ImageInfo image)
        {
            if (dbUserCertificate == null)
            {
                throw new ArgumentNullException(nameof(dbUserCertificate));
            }

            return new CertificateInfo
            {
                Id = dbUserCertificate.Id,
                Name = dbUserCertificate.Name,
                EducationType = (EducationType)dbUserCertificate.EducationType,
                ReceivedAt = dbUserCertificate.ReceivedAt,
                SchoolName = dbUserCertificate.SchoolName,
                Image = image
            };
        }
    }
}
