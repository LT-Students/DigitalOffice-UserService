using System;

namespace LT.DigitalOffice.Broker.Requests
{
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
