using LT.DigitalOffice.Broker.Responses;

namespace LT.DigitalOffice.UserService.Mappers.UnitTests.Utils
{
    /// <summary>
    /// DTO for mass transit. Class allows testing where needed an instance of IUserPositionResponse.
    /// </summary>
    public class InheritedUserPositionResponse : IUserPositionResponse
    {
        public string UserPositionName { get; set; }
    }
}