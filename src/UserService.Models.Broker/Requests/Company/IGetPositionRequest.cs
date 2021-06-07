using System;

namespace LT.DigitalOffice.Broker.Requests
{
    /// <summary>
    /// DTO for getting user position through a message broker.
    /// </summary>
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