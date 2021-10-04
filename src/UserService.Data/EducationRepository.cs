using LT.DigitalOffice.CompanyService.Data.Provider;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.UserService.Data
{
  public class EducationRepository : IEducationRepository
  {
    private readonly IDataProvider _provider;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<EducationRepository> _logger;

    public EducationRepository(
      IDataProvider provider,
      IHttpContextAccessor httpContextAccessor,
      ILogger<EducationRepository> logger)
    {
      _provider = provider;
      _httpContextAccessor = httpContextAccessor;
      _logger = logger;
    }

    public void Add(DbUserEducation education)
    {
      if (education == null)
      {
        _logger.LogWarning(new ArgumentNullException(nameof(education)).Message);
        return;
      }
      
      _provider.UserEducations.Add(education);
      _provider.Save();
    }

    public DbUserEducation Get(Guid educationId)
    {
      DbUserEducation education = _provider.UserEducations
        .FirstOrDefault(e => e.Id == educationId);

      if (education == null)
      {
        _logger.LogWarning($"User education with ID '{educationId}' was not found.");
      }

      return education;
    }

    public bool Edit(DbUserEducation education, JsonPatchDocument<DbUserEducation> request)
    {
      if (education == null)
      {
        _logger.LogWarning(new ArgumentNullException(nameof(education)).Message);
        return false;
      }

      if (request == null)
      {
        _logger.LogWarning(new ArgumentNullException(nameof(request)).Message);
        return false;
      }

      request.ApplyTo(education);
      education.ModifiedBy = _httpContextAccessor.HttpContext.GetUserId();
      education.ModifiedAtUtc = DateTime.UtcNow;
      _provider.Save();

      return true;
    }

    public bool Remove(DbUserEducation education)
    {
      if (education == null)
      {
        _logger.LogWarning(new ArgumentNullException(nameof(education)).Message);
        return false;
      }

      _provider.UserEducations.Remove(education);
      _provider.Save();

      return true;
    }
  }
}
