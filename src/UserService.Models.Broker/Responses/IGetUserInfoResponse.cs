using System;

namespace LT.DigitalOffice.Broker.Responses
{
    public interface IGetUserInfoResponse
    {
        Guid Id { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        //string MiddleName { get; set; }
        bool IsActive { get; set; }

        static object CreateObj(Guid id, string firstName, /*string middleName,*/ string lastName, bool isActive)
        {
            return new
            {
                Id = id,
                FirstName = firstName,
                LastName = lastName,
                //MiddleName = middleName,
                IsActive = isActive
            };
        }
    }
}
