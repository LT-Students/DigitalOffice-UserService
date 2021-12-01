using LT.DigitalOffice.Models.Broker.Models.Education;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using System;

namespace LT.DigitalOffice.UserService.Mappers.Models
{
  public class CertificateInfoMapper : ICertificateInfoMapper
  {
    public CertificateInfo Map(CertificateData certificateData)
    {
      if (certificateData is null)
      {
        return null;
      }

      return new CertificateInfo
      {
        Id = certificateData.Id,
        Name = certificateData.Name,
        EducationType = certificateData.EducationType,
        ReceivedAt = certificateData.ReceivedAt,
        SchoolName = certificateData.SchoolName
      };
    }
  }
}
