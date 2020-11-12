using System;
using System.Collections.Generic;
using System.Text;

namespace LT.DigitalOffice.Broker.Requests
{
    public interface IUserExistsAndActiveRequest
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
