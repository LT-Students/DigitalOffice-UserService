using System;

namespace LT.DigitalOffice.UserService.Broker.Requests
{
    public interface IUserDescriptionRequest
    {
        Guid GeneratedId { get; }
        string Email { get; }
        string FirstName { get; }
        string LastName { get; }
        string MiddleName { get; }
    }
}
