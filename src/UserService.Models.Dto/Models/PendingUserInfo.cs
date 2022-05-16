using System;

namespace LT.DigitalOffice.UserService.Models.Dto.Models
{
  public record PendingUserInfo
  {
    public Guid InvitationCommunicationId { get; set; }
  }
}
