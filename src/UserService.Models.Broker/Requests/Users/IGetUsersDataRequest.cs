using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.Broker.Requests
{
    public interface IGetUsersDataRequest
    {
        List<Guid> UserIds { get; }

        static object CreateObj(List<Guid> userIds)
        {
            return new
            {
                UserIds = userIds
            };
        }
    }
}
