using System;
using System.Collections.Generic;
using System.Text;

namespace LT.DigitalOffice.Broker.Responses
{
    public interface IUserExistsAndActiveResponse
    {
        bool IsUserExists { get; }
        bool? IsActive { get; }

        static object CreateObj(bool isUserExists, bool? isActive)
        {
            return new
            {
                IsUserExists = isUserExists,
                IsActive = isActive
            };
        }
    }
}
