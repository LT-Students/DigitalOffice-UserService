using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Helper.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Achievement;
using Microsoft.AspNetCore.Http;
using System;

namespace LT.DigitalOffice.UserService.Mappers.Db
{
  public class DbAchievementMapper : IDbAchievementMapper
  {
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IResizeImageHelper _resizeHelper;

    public DbAchievementMapper(IHttpContextAccessor httpContextAccessor, IResizeImageHelper resizeHelper)
    {
      _httpContextAccessor = httpContextAccessor;
      _resizeHelper = resizeHelper;
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
        ImageContent = request.Image != null ?
          _resizeHelper.Resize(request.Image.Content, request.Image.Extension) : null,
        Name = request.Name,
        Description = request.Description,
        IsActive = true,
        CreatedBy = _httpContextAccessor.HttpContext.GetUserId(),
        CreatedAtUtc = DateTime.UtcNow,
      };
    }
  }
}
