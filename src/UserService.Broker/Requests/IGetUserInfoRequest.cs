using System;

namespace UserService.Broker.Requests
{
    /// <summary>
    /// DTO for getting user information through a message broker.
    /// </summary>
    public interface IGetUserInfoRequest
    {
        Guid UserId { get; }
    }
}
