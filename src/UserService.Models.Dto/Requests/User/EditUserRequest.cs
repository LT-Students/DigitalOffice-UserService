using LT.DigitalOffice.UserService.Models.Dto.Enums;
using System;

namespace LT.DigitalOffice.UserService.Models.Dto.Requests.User
{
  public class EditUserRequest
  {
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MiddleName { get; set; }
    public bool IsAdmin { get; set; }
    public string About { get; set; }
    public Guid? GenderId { get; set; }
    public DateTime? DateOfBirth { get; set; } 
    public UserStatus Status { get; set; }     
    public bool IsActive { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public DateTime? BusinessHoursFromUtc { get; set; } 
    public DateTime? BusinessHoursToUtc { get; set; }   
  }
}