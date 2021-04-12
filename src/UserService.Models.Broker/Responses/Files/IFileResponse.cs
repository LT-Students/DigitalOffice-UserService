using System;

namespace LT.DigitalOffice.Broker.Responses
{
    /// <summary>
    /// Represents response for GetFileConsumer in MassTransit logic.
    /// </summary>
    public interface IFileResponse
    {
        Guid Id { get; }
        Guid? ParentId { get; set; }
        string Content { get; }
        string Extension { get; }
        string Name { get; }
    }
}