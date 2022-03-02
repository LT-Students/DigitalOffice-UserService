using System;

namespace LT.DigitalOffice.UserService.Models.Dto.Models
{
  public record PendingUserInfo
  {
    public UserInfo User { get; set; }
    public Guid InvintationCommunicationId { get; set; }
  }
}
