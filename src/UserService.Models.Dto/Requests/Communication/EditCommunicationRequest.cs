using LT.DigitalOffice.UserService.Models.Dto.Enums;

namespace LT.DigitalOffice.UserService.Models.Dto.Requests.Communication
{
  public record EditCommunicationRequest
  {
    public CommunicationType? Type { get; set; }
    public string Value { get; set; }
  }
}
