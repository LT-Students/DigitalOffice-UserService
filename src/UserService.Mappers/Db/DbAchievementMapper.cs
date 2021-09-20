using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Achievement;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Mappers.Db
{
  public class DbAchievementMapper : IDbAchievementMapper
  {
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DbAchievementMapper(IHttpContextAccessor httpContextAccessor)
    {
      _httpContextAccessor = httpContextAccessor;
    }

    public DbAchievement Map(CreateAchievementRequest request)
    {
      if (request == null)
      {
        return null;
      }
      return new DbAchievement
      {
        Id = Guid.NewGuid(),
        ImageId = imageId,
        Name = request.Name,
        Description = request.Description,
        IsActive = true,
        CreatedBy = _httpContextAccessor.HttpContext.GetUserId(),
        CreatedAtUtc = DateTime.UtcNow,
      };
    }
  }
}
