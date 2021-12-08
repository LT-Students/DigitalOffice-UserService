using System;

namespace LT.DigitalOffice.UserService.Models.Dto.Requests.User
{
  public class CreateGenderRequest
  {
    public Guid UserId { get; set; }
    public string Name { get; set; }
  }
}
