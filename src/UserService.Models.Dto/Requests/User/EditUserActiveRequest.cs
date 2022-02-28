using System;

namespace LT.DigitalOffice.UserService.Models.Dto.Requests.User
{
  public record EditUserActiveRequest
  {
    public Guid UserId { get; set; }
    public bool IsActive { get; set; }
    public Guid? CommunicationId { get; set; }
  }
}
