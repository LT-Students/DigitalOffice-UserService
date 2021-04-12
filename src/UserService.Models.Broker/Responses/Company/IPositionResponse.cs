using System;

namespace LT.DigitalOffice.Broker.Responses
{
    /// <summary>
    /// DTO for getting user position through a message broker.
    /// </summary>
    public interface IPositionResponse
    {
        Guid Id { get; }
        string Name { get; }
        DateTime ReceivedAt { get; }
    }
}