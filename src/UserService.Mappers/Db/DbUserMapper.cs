using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Models.Broker.Requests.User;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.UserService.Mappers.Db
{
  public class DbUserMapper : IDbUserMapper
  {
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DbUserMapper(IHttpContextAccessor httpContextAccessor)
    {
      _httpContextAccessor = httpContextAccessor;
    }

    public DbUser Map(CreateUserRequest request)
    {
      if (request is null)
      {
        return null;
      }

      Guid userId = Guid.NewGuid();
      Guid createdBy = _httpContextAccessor.HttpContext.GetUserId();
      DateTime createdAtUtc = DateTime.UtcNow;

      DbUser dbUser = new()
      {
        Id = userId,
        FirstName = request.FirstName,
        LastName = request.LastName,
        MiddleName = !string.IsNullOrEmpty(request.MiddleName?.Trim()) ? request.MiddleName.Trim() : null,
        Status = (int)request.Status,
        IsAdmin = request.IsAdmin ?? false,
        IsActive = false,
        CreatedBy = createdBy,
        CreatedAtUtc = createdAtUtc,
        Communications = new List<DbUserCommunication> {
          new DbUserCommunication
          {
            Id = Guid.NewGuid(),
            Type = (int)request.Communication.Type,
            Value = request.Communication.Value,
            UserId = userId,
            CreatedBy = createdBy,
            CreatedAtUtc = createdAtUtc
          }
        },
        Addition = new DbUserAddition
        {
          Id = Guid.NewGuid(),
          UserId = userId,
          GenderId = null,
          About = request.About,
          DateOfBirth = request.DateOfBirth,
          Latitude = request.Latitude,
          Longitude = request.Longitude,
          BusinessHoursFromUtc = request.BusinessHoursFromUtc,
          BusinessHoursToUtc = request.BusinessHoursToUtc,
          ModifiedBy = createdBy,
          ModifiedAtUtc = createdAtUtc
        }
      };

      return dbUser;
    }

    public DbUser Map(ICreateAdminRequest request)
    {
      if (request == null)
      {
        return null;
      }

      Guid userId = Guid.NewGuid();
      DateTime createdAtUtc = DateTime.UtcNow;

      return new DbUser
      {
        Id = userId,
        FirstName = request.FirstName,
        LastName = request.LastName,
        MiddleName = !string.IsNullOrEmpty(request.MiddleName) ? request.MiddleName : null,
        Status = (int)UserStatus.WorkFromOffice,
        IsActive = true,
        IsAdmin = true,
        CreatedBy = userId,
        CreatedAtUtc = createdAtUtc,
        Communications = new List<DbUserCommunication> {
          new DbUserCommunication
          {
            Id = Guid.NewGuid(),
            Type = (int)CommunicationType.Email,
            Value = request.Email,
            UserId = userId,
            CreatedBy = userId,
            CreatedAtUtc = createdAtUtc
          }
        },
        Addition = new DbUserAddition
        {
          Id = Guid.NewGuid(),
          UserId = userId,
          GenderId = null,
          About = null,
          DateOfBirth = null,
          Latitude = null,
          Longitude = null,
          BusinessHoursFromUtc = null,
          BusinessHoursToUtc = null,
          ModifiedBy = userId,
          ModifiedAtUtc = createdAtUtc
        }
      };
    }
  }
}