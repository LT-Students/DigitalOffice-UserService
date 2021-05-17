using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Dto.Configurations;
using System;

namespace LT.DigitalOffice.Broker.Requests
{
    [AutoInjectRequest(nameof(RabbitMqConfig.AddImageEndpoint))]
    public interface IAddImageRequest
    {
        string Name { get; set; }
        string Content { get; set; }
        string Extension { get; set; }
        Guid UserId { get; set; }

        static object CreateObj(
            string name,
            string content,
            string extension,
            Guid userId)
        {
            return new
            {
                Name = name,
                Content = content,
                Extension = extension,
                UserId = userId
            };
        }
    }
}
