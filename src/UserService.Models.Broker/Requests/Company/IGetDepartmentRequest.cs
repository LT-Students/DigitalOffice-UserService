using System;

namespace LT.DigitalOffice.Broker.Requests
{
    public interface IGetDepartmentRequest
    {
        Guid? UserId { get; }
        Guid? DepartmentId { get; }

        static object CreateObj(Guid? userId, Guid? departmentId)
        {
            return new
            {
                UserId = userId,
                DepartmentId = departmentId
            };
        }
    }
}
