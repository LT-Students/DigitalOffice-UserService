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
                Rate = request.Rate,
                StartWorkingAt = request.StartWorkingAt,
                DateOfBirth = request.DateOfBirth,
                CreatedBy = _httpContextAccessor.HttpContext.GetUserId(),
                CreatedAtUtc = DateTime.UtcNow,
                Communications = request.Communications?.Select(x => new DbUserCommunication
                {
                    Id = Guid.NewGuid(),
                    Type = (int)x.Type,
                    Value = x.Value,
                    UserId = userId
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
                Rate = 1,
                CreatedBy = userId,
                CreatedAtUtc = DateTime.UtcNow,
                Communications = new List<DbUserCommunication> {
                    new DbUserCommunication
                    {
                        Id = Guid.NewGuid(),
                        Type = (int)CommunicationType.Email,
                        Value = request.Email,
                        UserId = userId
                    }
                }
            };
        }
    }
}