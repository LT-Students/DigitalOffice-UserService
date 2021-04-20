using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using System;
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

            string salt = $"{Guid.NewGuid()}{Guid.NewGuid()}";

            if (!DateTime.TryParse(request.StartWorkingAt, out DateTime startWorkingAt))
            {
                throw new BadRequestException(
                    $"You must specify '{nameof(CreateUserRequest.StartWorkingAt)}' in format 'YYYY-MM-DD'");
            }

            return new DbUser
            {
                Id = userId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                MiddleName = request.MiddleName,
                Status = (int)request.Status,
                AvatarFileId = avatarImageId,
                IsActive = false,
                IsAdmin = request.IsAdmin ?? false,
                CreatedAt = DateTime.UtcNow,
                StartWorkingAt = startWorkingAt,
                Rate = request.Rate,
                Communications = request.Communications?.Select(x => new DbUserCommunication
                {
                    Id = Guid.NewGuid(),
                    Type = (int)x.Type,
                    Value = x.Value,
                    UserId = userId
                }).ToList()
            };
        }
    }
}