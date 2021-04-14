using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using MassTransit;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.UserService.Mappers.Models
{
    public class PatchDbUserMapper : IPatchDbUserMapper
    {
        private readonly ILogger<PatchDbUserMapper> _logger;
        private readonly ICertificateInfoMapper _certificateMapper;
        private readonly IRequestClient<IAddImageRequest> _requestClient;

        private Guid? AddImage(string image)
        {
            Guid? avatarImageId = null;
            if (!string.IsNullOrEmpty(image))
            {
                try
                {
                    Response<IOperationResult<Guid>> response = _requestClient.GetResponse<IOperationResult<Guid>>(
                        IAddImageRequest.CreateObj(image),
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

        public PatchDbUserMapper(
            ILogger<PatchDbUserMapper> logger,
            ICertificateInfoMapper certificateMapper,
            IRequestClient<IAddImageRequest> requestClient)
        {
            _logger = logger;
            _requestClient = requestClient;
            _certificateMapper = certificateMapper;
        }

        public JsonPatchDocument<DbUser> Map(JsonPatchDocument<EditUserRequest> request, Guid userId)
        {
            if (request == null)
            {
                throw new BadRequestException();
            }

            var result = new JsonPatchDocument<DbUser>();

            foreach (var item in request.Operations)
            {
                if (string.Equals(
                    item.path,
                    $"/{nameof(EditUserRequest.AvatarImage)}",
                    StringComparison.OrdinalIgnoreCase))
                {
                    item.path = $"/{nameof(DbUser.AvatarFileId)}";
                    item.value = AddImage(item.value.ToString());
                }

                if (string.Equals(
                    item.path,
                    $"/{nameof(EditUserRequest.Certificates)}/-",
                    StringComparison.OrdinalIgnoreCase))
                {
                    item.path = $"/{nameof(DbUser.Certificates)}/-";
                    var certificates = (List<CertificateInfo>)item.value;

                    item.value = certificates.Select(x => new DbUserCertificate
                    {
                        Id = x.Id ?? Guid.NewGuid(),
                        UserId = userId,
                        ImageId = x.Image.Id ?? (Guid)AddImage(x.Image.Content),
                        EducationType = (int)x.Type,
                        Name = x.Name,
                        SchoolName = x.SchoolName,
                        ReceivedAt = DateTime.UtcNow
                    });
                }

                if (string.Equals(
                    item.path,
                    $"/{nameof(EditUserRequest.Status)}",
                    StringComparison.OrdinalIgnoreCase))
                {
                    item.value = (int) item.value;
                }

                string operation = item.OperationType switch
                {
                    OperationType.Add => "/add",
                    OperationType.Remove => "/remove",
                    OperationType.Replace => "/replace",
                    _ => null
                };

                if (operation != null)
                {
                    result.Operations.Add(new Operation<DbUser>(operation, item.path, item.from, item.value));
                }
            }

            return result;
        }
    }
}