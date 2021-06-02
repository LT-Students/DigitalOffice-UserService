using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Dto.Configurations;
using System;

namespace LT.DigitalOffice.Broker.Requests
{
    [AutoInjectRequest(nameof(RabbitMqConfig.ChangeUserDepartmentEndpoint))]
    public interface IChangeUserDepartmentRequest
    {
        Guid UserId { get; }
        Guid DepartmentId { get; }

        static object CreateObj(Guid userId, Guid departmentId)
        {
            return new
            {
                UserId = userId,
                DepartmentId = departmentId
            };
        }
    }
}
