using LT.DigitalOffice.CompanyService.Data.Provider;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.UserService.Data
{
  public class EducationRepository : IEducationRepository
  {
    private readonly IDataProvider _provider;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public EducationRepository(
    IDataProvider provider,
    IHttpContextAccessor httpContextAccessor)
    {
      _provider = provider;
      _httpContextAccessor = httpContextAccessor;
    }

    public void Add(DbUserEducation education)
    {
      if (education == null)
      {
        throw new ArgumentNullException(nameof(education));
      }

      ICollection<DbUserEducationImage> userEducationImages = education.Images;

      if (userEducationImages.Any())
      {
        _provider.UserEducationImages.AddRange(userEducationImages);
      }
      
      _provider.UserEducations.Add(education);
      _provider.Save();
    }

    public DbUserEducation Get(Guid educationId)
    {
      DbUserEducation education = _provider.UserEducations
        .Include(e => e.Images)
        .FirstOrDefault(e => e.Id == educationId);

      if (education == null)
      {
        throw new NotFoundException($"User education with ID '{educationId}' was not found.");
      }

      return education;
    }

    public bool Edit(DbUserEducation education, JsonPatchDocument<DbUserEducation> request)
    {
      if (education == null)
      {
        throw new ArgumentNullException(nameof(education));
      }

      if (request == null)
      {
        throw new ArgumentNullException(nameof(request));
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
        throw new ArgumentNullException(nameof(education));
      }

      _provider.UserEducationImages.RemoveRange(education.Images);
      _provider.UserEducations.Remove(education);
      _provider.Save();

      return true;
    }
  }
}
