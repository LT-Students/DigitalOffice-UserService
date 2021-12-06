using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using Microsoft.AspNetCore.Http;
using System;

namespace LT.DigitalOffice.UserService.Mappers.Db
{
  public class DbGenderMapper : IDbGenderMapper
  {
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DbGenderMapper(IHttpContextAccessor httpContextAccessor)
    {
      _httpContextAccessor = httpContextAccessor;
    }

    public DbGender Map(CreateGenderRequest request)
    {
      if (request == null)
      {
        return null;
      }

      return new DbGender
      {
        Id = Guid.NewGuid(),
        Name = request.Name.Trim(),
        CreatedBy = _httpContextAccessor.HttpContext.GetUserId(),
        CreatedAtUtc = DateTime.UtcNow,
      };
    }
  }
}
