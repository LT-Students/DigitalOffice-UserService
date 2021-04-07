using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Dto.Configurations;
using System;

namespace LT.DigitalOffice.Broker.Requests
{
    [AutoInjectRequest(nameof(RabbitMqConfig.GetFileEndpoint))]
    public interface IGetFileRequest
    {
        Guid Id { get; }
        bool IsImage { get; }

        static object CreateObj(Guid id, bool isImage)
        {
            return new
            {
                Id = id,
                IsImage = isImage
            };
        }
    }
}
