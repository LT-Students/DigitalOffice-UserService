namespace LT.DigitalOffice.Broker.Requests
{
    /// <summary>
    /// The model is a binding the request internal model of sender for RabbitMQ.
    /// </summary>
    public interface IUserCredentialsRequest
    {
        string LoginData { get; }

        static object CreateObj(string loginData)
        {
            return new
            {
                LoginData = loginData
            };
        }
    }
}