using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Dto.Configurations;
using System;

namespace LT.DigitalOffice.Broker.Requests
{
    [AutoInjectRequest(nameof(RabbitMqConfig.ChangeUserPositionEndpoint))]
    public interface IChangeUserPositionRequest
    {
        Guid UserId { get; }
        Guid PositionId { get; }

        static object CreateObj(Guid userId, Guid positionId)
        {
            return new
            {
                UserId = userId,
                PositionId = positionId
            };
        }
    }
}
