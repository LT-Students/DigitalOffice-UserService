using LT.DigitalOffice.Kernel.Exceptions;
using LT.DigitalOffice.UserService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.UserService.Mappers.RequestsMappers
{
    public class UserRequestMapper : IUserRequestMapper
    {
        public DbUser Map(UserRequest value)
        {
            if (value == null)
            {
                throw new BadRequestException();
            }

            value.Id ??= Guid.NewGuid();

            return new DbUser
            {
                Id = value.Id.Value,
                Email = value.Email,
                FirstName = value.FirstName,
                LastName = value.LastName,
                MiddleName = value.MiddleName,
                Status = value.Status,
                AvatarFileId = value.AvatarFileId,
                IsActive = value.IsActive,
                IsAdmin = value.IsAdmin
            };
        }
    }
}