using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Mappers.Models
{
    public class CertificateInfoMapper : ICertificateInfoMapper
    {
        public CertificateInfo Map(
            DbUserCertificate dbUserCertificate,
            List<ImageInfo> images)
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
                Images = images
            };
        }
    }
}
