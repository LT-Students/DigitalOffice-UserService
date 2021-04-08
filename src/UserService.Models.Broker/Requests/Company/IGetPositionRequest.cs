using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Dto.Configurations;
using System;

namespace LT.DigitalOffice.Broker.Requests
{
    /// <summary>
    /// DTO for getting user position through a message broker.
    /// </summary>
    [AutoInjectRequest(nameof(RabbitMqConfig.GetPositionEndpoint))]
    public interface IGetPositionRequest
    {
        Guid? UserId { get; }
        Guid? PositionId { get; set; }

        static object CreateObj(Guid? userId, Guid? positionId)
        {
            return new
            {
                UserId = userId,
                PositionId = positionId
            };
        }
    }
}