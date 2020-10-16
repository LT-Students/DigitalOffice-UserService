using System;

namespace LT.DigitalOffice.Broker.Requests
{
    /// <summary>
    /// DTO for getting user position through a message broker.
    /// </summary>
    public interface IGetUserPositionRequest
    {
        Guid UserId { get; }
    }
}