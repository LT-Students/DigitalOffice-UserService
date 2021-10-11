using LT.DigitalOffice.CompanyService.Data.Provider;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Linq;
using System.Threading.Tasks;

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

    public async Task Add(DbUserEducation education)
    {
      if (education == null)
      {
        throw new ArgumentNullException(nameof(education));
      }

      _provider.UserEducations.Add(education);
      await _provider.SaveAsync();
    }

    public DbUserEducation Get(Guid educationId)
    {
      DbUserEducation education = _provider.UserEducations.FirstOrDefault(e => e.Id == educationId);

      if (education == null)
      {
        throw new NotFoundException($"User education with ID '{educationId}' was not found.");
      }

      return education;
    }

    public async Task<bool> Edit(DbUserEducation education, JsonPatchDocument<DbUserEducation> request)
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
      await _provider.SaveAsync();

      return true;
    }

    public async Task<bool> Remove(DbUserEducation education)
    {
      if (education == null)
      {
        throw new ArgumentNullException(nameof(education));
      }

      education.IsActive = false;
      education.ModifiedBy = _httpContextAccessor.HttpContext.GetUserId();
      education.ModifiedAtUtc = DateTime.UtcNow;
      await _provider.SaveAsync();

      return true;
    }
  }
}
