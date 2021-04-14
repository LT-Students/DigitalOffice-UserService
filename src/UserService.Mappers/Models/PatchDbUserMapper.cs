using System;
using System.Net;
using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using MassTransit;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.UserService.Mappers.Models
{
    public class PatchDbUserMapper : IPatchDbUserMapper
    {
        private readonly ILogger<PatchDbUserMapper> _logger;
        private readonly IRequestClient<IAddImageRequest> _requestClient;

        public PatchDbUserMapper(
            ILogger<PatchDbUserMapper> logger,
            IRequestClient<IAddImageRequest> requestClient)
        {
            _logger = logger;
            _requestClient = requestClient;
        }
        
        public JsonPatchDocument<DbUser> Map(
            JsonPatchDocument<EditUserRequest> request,
            Func<string, Guid?> getAvatarImageId)
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
                    item.value = getAvatarImageId.Invoke(item.value.ToString());
                    
                    if (item.value == null)
                    {
                        continue;
                    }
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
                    OperationType.Add => "add",
                    OperationType.Remove => "remove",
                    OperationType.Replace => "replace",
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