using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Communication;
using Microsoft.AspNetCore.Http;
using System;

namespace LT.DigitalOffice.UserService.Mappers.Db
{
  public class DbUserCommunicationMapper : IDbUserCommunicationMapper
  {
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DbUserCommunicationMapper(IHttpContextAccessor httpContextAccessor)
    {
      _httpContextAccessor = httpContextAccessor;
    }

    public DbUserCommunication Map(CreateCommunicationRequest request, Guid? userId = null)
    {
      return request is null
        ? null
        : new DbUserCommunication
        {
          Id = Guid.NewGuid(),
          UserId = request.UserId.HasValue ? request.UserId.Value : userId.Value,
          Type = (int)request.Type,
          Value = request.Value,
          CreatedBy = _httpContextAccessor.HttpContext.GetUserId(),
          CreatedAtUtc = DateTime.UtcNow,
        };
    }
  }
}
