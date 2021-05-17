using System;

namespace LT.DigitalOffice.Broker.Responses
{
    public interface IAddImageResponse
    {
        Guid Id { get; }

        static object CreateObj(Guid id)
        {
            return new
            {
                Id = id
            };
        }
    }
}