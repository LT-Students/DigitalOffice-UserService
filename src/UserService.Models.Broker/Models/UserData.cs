using System;

namespace LT.DigitalOffice.UserService.Models.Broker.Models
{
    public class UserData
    {
        Guid Id { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string MiddleName { get; set; }
        bool IsActive { get; set; }

        public static UserData Create(Guid id, string firstName, string middleName, string lastName, bool isActive)
        {
            return new UserData
            {
                Id = id,
                FirstName = firstName,
                LastName = lastName,
                MiddleName = middleName,
                IsActive = isActive
            };
        }
    }
}
