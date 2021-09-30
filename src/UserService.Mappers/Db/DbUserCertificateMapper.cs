using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Certificates;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.UserService.Mappers.Db
{
  public class DbUserCertificateMapper : IDbUserCertificateMapper
  {
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DbUserCertificateMapper(IHttpContextAccessor httpContextAccessor)
    {
      _httpContextAccessor = httpContextAccessor;
    }

    public DbUserCertificate Map(CreateCertificateRequest request, List<Guid> imagesIds)
    {
      if (request == null)
      {
        return null;
      }

      Guid Id = Guid.NewGuid();

      return new DbUserCertificate
      {
        Id = Id,
        UserId = request.UserId,
        Images = imagesIds?.Select(imageId => new DbUserCertificateImage()
        {
          ImageId = imageId,
          UserCertificateId = Id
        }).ToList(),
        Name = request.Name,
        SchoolName = request.SchoolName,
        EducationType = (int)request.EducationType,
        ReceivedAt = request.ReceivedAt,
        IsActive = true,
        CreatedBy = _httpContextAccessor.HttpContext.GetUserId(),
        CreatedAtUtc = DateTime.UtcNow,
      };
    }
  }
}
