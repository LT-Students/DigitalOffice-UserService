using System;

namespace LT.DigitalOffice.Broker.Requests
{
    public interface IGetFileRequest
    {
        Guid Id { get; }
        bool IsImage { get; }

        static object CreateObj(Guid id, bool isImage)
        {
            return new
            {
                Id = id,
                IsImage = isImage
            };
        }
    }
}
