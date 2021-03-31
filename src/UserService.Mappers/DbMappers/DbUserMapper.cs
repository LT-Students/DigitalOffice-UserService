using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.UserService.Mappers.DbMappers.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace LT.DigitalOffice.UserService.Mappers.DbMappers
{
    public class DbUserMapper : IDbUserMapper
    {
        private readonly ILogger<DbUserMapper> _logger;
        private readonly IRequestClient<IAddImageRequest> _requestClient;

        private Guid? GetAvatarImageId(CreateUserRequest request)
        {
            Guid? avatarImageId = null;
            if (!string.IsNullOrEmpty(request.AvatarImage))
            {
                try
                {
                    Response<IOperationResult<Guid>> response = _requestClient.GetResponse<IOperationResult<Guid>>(
                        IAddImageRequest.CreateObj(request.AvatarImage),
                        timeout: RequestTimeout.After(ms: 500)).Result;

                    if (!response.Message.IsSuccess)
                    {
                        _logger.LogWarning($"Can not add avatar image. Reason: '{string.Join(',', response.Message.Errors)}'");
                    }
                    else
                    {
                        avatarImageId = response.Message.Body;
                    }
                }
                catch (Exception exc)
                {
                    _logger.LogError(exc, "Exception while add avatar image to db.");
                }
            }

            return avatarImageId;
        }

        public DbUserMapper(
            ILogger<DbUserMapper> logger,
            IRequestClient<IAddImageRequest> requestClient)
        {
            _logger = logger;
            _requestClient = requestClient;
        }

        public DbUser Map(CreateUserRequest request, Func<string, string, string, string> func)
        {
            if (request == null)
            {
                throw new BadRequestException();
            }

            Guid? avatarImageId = GetAvatarImageId(request);

            Guid userId = Guid.NewGuid();

            string salt = $"{ Guid.NewGuid() }{ Guid.NewGuid() }";

            return new DbUser
            {
                Id = userId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                MiddleName = request.MiddleName,
                Status = (int)request.Status,
                AvatarFileId = avatarImageId,
                IsActive = true,
                IsAdmin = request.IsAdmin,
                CreatedAt = DateTime.UtcNow,
                Credentials = new DbUserCredentials
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Login = request.Login,
                    Salt = salt,
                    PasswordHash = func.Invoke(request.Login, salt, request.Password),
                },
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