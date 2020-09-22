using System;

namespace LT.DigitalOffice.Broker.Responses
{
    /// <summary>
    /// DTO for dispatch user information through a message broker.
    /// </summary>
    public interface IUserInfoResponse
    {
        public Guid UserId { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string MiddleName { get; }
        public IUserPositionResponse UserPosition { get; }
    }
}