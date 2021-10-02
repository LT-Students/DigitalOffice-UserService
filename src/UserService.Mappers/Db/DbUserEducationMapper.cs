using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Education;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.UserService.Mappers.Db
{
  public class DbUserEducationMapper : IDbUserEducationMapper
  {
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DbUserEducationMapper(IHttpContextAccessor httpContextAccessor)
    {
      _httpContextAccessor = httpContextAccessor;
    }

    public DbUserEducation Map(CreateEducationRequest request)
    {
      if (request == null)
      {
        return null;
      }

      return new DbUserEducation
      {
        Id = Guid.NewGuid(),
        UserId = request.UserId,
        UniversityName = request.UniversityName,
        QualificationName = request.QualificationName,
        AdmissionAt = request.AdmissionAt,
        IssueAt = request.IssueAt,
        FormEducation = (int)request.FormEducation,
        IsActive = true,
        CreatedBy = _httpContextAccessor.HttpContext.GetUserId(),
        CreatedAtUtc = DateTime.UtcNow,
      };
    }
  }
}
