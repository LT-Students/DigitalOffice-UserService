using System;

namespace LT.DigitalOffice.Broker.Responses
{
    public interface IDepartmentResponse
    {
        Guid Id { get; }
        string Name { get; }
        DateTime StartWorkingAt { get; }
    }
}
