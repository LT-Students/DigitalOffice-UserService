namespace LT.DigitalOffice.Broker.Requests
{
    public interface IAddImageRequest
    {
        string Content { get; }

        static object CreateObj(string content)
        {
            return new
            {
                Content = content
            };
        }
    }
}
