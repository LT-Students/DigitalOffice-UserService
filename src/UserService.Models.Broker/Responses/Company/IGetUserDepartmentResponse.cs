using System;

namespace LT.DigitalOffice.Broker.Responses
{
    public interface IGetUserDepartmentResponse
    {
        Guid DepartmentId { get; }
        string Name { get; }
        DateTime StartWorkingAt { get; }
    }
}
