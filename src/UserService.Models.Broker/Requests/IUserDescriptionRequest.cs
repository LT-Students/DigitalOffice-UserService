using System;

namespace LT.DigitalOffice.Broker.Requests
{
    /// <summary>
    /// DTO for sent user information through a message broker in MessageService.
    /// </summary>
    public interface IUserDescriptionRequest
    {
        Guid GeneratedId { get; }
        string Email { get; }
        string FirstName { get; }
        string LastName { get; }
        string MiddleName { get; }
    }
}
