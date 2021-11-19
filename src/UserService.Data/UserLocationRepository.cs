using LT.DigitalOffice.CompanyService.Data.Provider;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Data
{
  public class UserLocationRepository : IUserLocationRepository
  {
    private readonly IDataProvider _provider;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserLocationRepository(
      IDataProvider provider,
      IHttpContextAccessor httpContextAccessor)
    {
      _provider = provider;
      _httpContextAccessor = httpContextAccessor;
    }

    private async Task<bool> CreateAsync(Guid userId, JsonPatchDocument<DbUserLocation> patch)
    {
      DbUserLocation dbUserLocation = new()
      {
        Id = Guid.NewGuid(),
        UserId = userId,
        CreatedAtUtc = DateTime.Now,
        CreatedBy = _httpContextAccessor.HttpContext.GetUserId()
      };

      patch.ApplyTo(dbUserLocation);

      _provider.UsersLocations.Add(dbUserLocation);
      await _provider.SaveAsync();

      return true;
    }

    public async Task<bool> EditAsync(Guid userId, JsonPatchDocument<DbUserLocation> patch)
    {
      if (patch is null)
      {
        return false;
      }

      DbUserLocation dbUserLocation = await _provider.UsersLocations.FirstOrDefaultAsync(ul => ul.UserId == userId);

      if (dbUserLocation is null)
      {
        return await CreateAsync(userId, patch);
      }

      patch.ApplyTo(dbUserLocation);
      dbUserLocation.ModifiedBy = _httpContextAccessor.HttpContext.GetUserId();
      dbUserLocation.ModifiedAtUtc = DateTime.Now;
      await _provider.SaveAsync();

      return true;
    }
  }
}
