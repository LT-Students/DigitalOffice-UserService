using System;

namespace LT.DigitalOffice.UserService.Models.Broker.Requests.Company
{
    public interface IUsersRequest
    {
        Guid DepartmentId { get; }
        int SkipCount { get; set; }
        int TakeCount { get; set; }

        static object CreateObj(Guid departmentId, int skipCount, int takeCount)
        {
            return new
            {
                DepartmentId = departmentId,
                SkipCount = skipCount,
                TakeCount = takeCount
            };
        }
    }
}
