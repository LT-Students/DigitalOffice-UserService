using LT.DigitalOffice.UserService.Models.Dto.Enums;
using System;

namespace LT.DigitalOffice.UserService.Models.Dto.Requests.User
{
  public class EditUserRequest
  {
    public Guid RoleId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MiddleName { get; set; }
    public string About { get; set; }
    public UserGender Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string City { get; set; }
    public UserStatus Status { get; set; }
    public DateTime? StartWorkingAt { get; set; }
    public bool IsActive { get; set; }
  }
}