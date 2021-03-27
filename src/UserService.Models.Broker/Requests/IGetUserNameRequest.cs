using System;

namespace LT.DigitalOffice.Broker.Requests
{
    public interface IGetUserNameRequest
    {
        public Guid UserId { get; set; }
    }
}