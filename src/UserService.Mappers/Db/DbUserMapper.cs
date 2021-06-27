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

            DbUser dbUser = new()
            {
                Id = userId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                MiddleName = request.MiddleName,
                City = request.City,
                Gender = (int)request.Gender,
                Status = (int)request.Status,
                AvatarFileId = avatarImageId,
                IsActive = false,
                IsAdmin = request.IsAdmin ?? false,
                CreatedAt = DateTime.UtcNow,
                Rate = request.Rate,
                Communications = request.Communications?.Select(x => new DbUserCommunication
                {
                    Id = Guid.NewGuid(),
                    Type = (int)x.Type,
                    Value = x.Value,
                    UserId = userId
                }).ToList()
            };

            string salt = $"{Guid.NewGuid()}{Guid.NewGuid()}";

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

            if (request.DayOfBirth != null)
            {
                if (DateTime.TryParse(request.DayOfBirth, out DateTime dayOfBirth))
                {
                    dbUser.DateOfBirth = dayOfBirth;
                }
                else
                {
                    throw new BadRequestException(
                        $"You must specify '{nameof(CreateUserRequest.DayOfBirth)}' in format 'YYYY-MM-DD'");
                }
            }

            return dbUser;
        }
    }
}