using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Models.Broker.Requests.User;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.UserService.Mappers.Db
{
    public class DbUserMapper : IDbUserMapper
    {
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
                FirstName = request.FirstName,
                LastName = request.LastName,
                MiddleName = request.MiddleName,
                Gender = (int)request.Gender,
                City = request.City,
                AvatarFileId = avatarImageId,
                Status = (int)request.Status,
                IsAdmin = request.IsAdmin ?? false,
                IsActive = false,
                Rate = request.Rate,
                CreatedAt = DateTime.UtcNow,
                Communications = request.Communications?.Select(x => new DbUserCommunication
                {
                    Id = Guid.NewGuid(),
                    Type = (int)x.Type,
                    Value = x.Value,
                    UserId = userId
                }).ToList()
            };

            if (request.StartWorkingAt != null)
            {
                if (DateTime.TryParse(request.StartWorkingAt, out DateTime startWorkingAt))
                {
                    dbUser.StartWorkingAt = startWorkingAt;
                }
                else {
                    throw new BadRequestException(
                        $"You must specify '{nameof(CreateUserRequest.StartWorkingAt)}' in format 'YYYY-MM-DD'");
                }

            }

            if (request.DateOfBirth != null)
            {
                if (DateTime.TryParse(request.DateOfBirth, out DateTime dayOfBirth))
                {
                    dbUser.DateOfBirth = dayOfBirth;
                }
                else
                {
                    throw new BadRequestException(
                        $"You must specify '{nameof(CreateUserRequest.DateOfBirth)}' in format 'YYYY-MM-DD'");
                }
            }

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
                MiddleName = request.MiddleName,
                Status = (int)UserStatus.WorkFromOffice,
                AvatarFileId = null,
                IsActive = true,
                IsAdmin = true,
                CreatedAt = DateTime.UtcNow,
                Rate = 1,
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