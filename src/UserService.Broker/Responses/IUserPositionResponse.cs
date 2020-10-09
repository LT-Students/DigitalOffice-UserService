namespace LT.DigitalOffice.Broker.Responses
{
    /// <summary>
    /// DTO for getting user position through a message broker.
    /// </summary>
    public interface IUserPositionResponse
    {
        string UserPositionName { get; }
    }
}