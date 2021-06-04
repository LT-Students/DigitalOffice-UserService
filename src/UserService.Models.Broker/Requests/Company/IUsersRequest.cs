using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Dto.Configurations;
using System;

namespace LT.DigitalOffice.UserService.Models.Broker.Requests.Company
{
    [AutoInjectRequest(nameof(RabbitMqConfig.GetDepartmentEndpoint))]
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
