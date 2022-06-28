using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Models.Broker.Requests.User;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Mappers.Db
{
  public class DbUserMapper : IDbUserMapper
  {
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDbUserCommunicationMapper _dbUserCommunicationMapper;

    public DbUserMapper(
      IHttpContextAccessor httpContextAccessor,
      IDbUserCommunicationMapper dbUserCommunicationMapper)
    {
      _httpContextAccessor = httpContextAccessor;
      _dbUserCommunicationMapper = dbUserCommunicationMapper;
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

      return new DbUser()
      {
        Id = userId,
        FirstName = request.FirstName,
        LastName = request.LastName,
        MiddleName = !string.IsNullOrEmpty(request.MiddleName?.Trim()) ? request.MiddleName.Trim() : null,
        IsAdmin = request.IsAdmin,
        IsActive = false,
        CreatedBy = createdBy,
        Communications = 
          new List<DbUserCommunication> { _dbUserCommunicationMapper.Map(request.Communication, userId) },
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
    }

    public DbUser Map(ICreateAdminRequest request)
    {
      if (request is null)
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
        IsActive = true,
        IsAdmin = true,
        CreatedBy = userId,
        Communications = new List<DbUserCommunication> {
          new DbUserCommunication
          {
            Id = Guid.NewGuid(),
            Type = (int)CommunicationType.BaseEmail,
            Value = request.Email,
            IsConfirmed = true,
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
          BusinessHoursFromUtc = null,
          BusinessHoursToUtc = null,
          Latitude = null,
          Longitude = null,
          ModifiedBy = userId,
          ModifiedAtUtc = createdAtUtc
        }
      };
    }
  }
}