using System;

namespace LT.DigitalOffice.Broker.Responses
{
    public interface IProjectResponse
    {
        Guid Id { get; }
        string Name { get; }
        bool IsActive { get; }
    }
}
