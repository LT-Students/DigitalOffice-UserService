using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Models.Broker.Requests.User;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.Kernel.Extensions;
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

        public DbUser Map(CreateUserRequest request, Guid? avatarImageId)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            Guid userId = Guid.NewGuid();
            Guid createdBy = _httpContextAccessor.HttpContext.GetUserId();
            DateTime createdAtUtc = DateTime.UtcNow;

            DbUser dbUser = new()
            {
                Id = userId,
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                MiddleName = !string.IsNullOrEmpty(request.MiddleName?.Trim()) ? request.MiddleName.Trim() : null,
                Gender = (int)request.Gender,
                City = !string.IsNullOrEmpty(request.City?.Trim()) ? request.City.Trim() : null,
                AvatarFileId = avatarImageId,
                Status = (int)request.Status,
                IsAdmin = request.IsAdmin ?? false,
                IsActive = false,
                StartWorkingAt = request.StartWorkingAt,
                DateOfBirth = request.DateOfBirth,
                CreatedBy = createdBy,
                CreatedAtUtc = createdAtUtc,
                Communications = request.Communications?.Select(x => new DbUserCommunication
                {
                    Id = Guid.NewGuid(),
                    Type = (int)x.Type,
                    Value = x.Value,
                    UserId = userId,
                    CreatedBy = createdBy,
                    CreatedAtUtc = createdAtUtc
                }).ToList()
            };

            return dbUser;
        }

        public DbUser Map(ICreateAdminRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            Guid userId = Guid.NewGuid();
            DateTime createdAtUtc = DateTime.UtcNow;

            return new DbUser
            {
                Id = userId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                MiddleName = !string.IsNullOrEmpty(request.MiddleName?.Trim()) ? request.MiddleName.Trim() : null,
                Status = (int)UserStatus.WorkFromOffice,
                AvatarFileId = null,
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
                }
            };
        }
    }
}