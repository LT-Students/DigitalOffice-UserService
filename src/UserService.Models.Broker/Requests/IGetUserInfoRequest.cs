using System;

namespace LT.DigitalOffice.Broker.Requests
{
    public interface IGetUserInfoRequest
    {
        Guid UserId { get; }

        static object CreateObj(Guid userId)
        {
            return new
            {
                UserId = userId
            };
        }
    }
}
