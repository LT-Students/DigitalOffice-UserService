using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Avatar;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Communication;
using LT.DigitalOffice.UserService.Models.Dto.Requests.UserCompany;
using System;
using System.ComponentModel.DataAnnotations;

namespace LT.DigitalOffice.UserService.Models.Dto
{
  public record CreateUserRequest
  {
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    public string MiddleName { get; set; }
    public bool IsAdmin { get; set; } = false;
    public string About { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public DateTime? BusinessHoursFromUtc { get; set; }
    public DateTime? BusinessHoursToUtc { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? OfficeId { get; set; }
    public Guid? PositionId { get; set; }
    public Guid? RoleId { get; set; }
    public string Password { get; set; }
    public CreateUserCompanyRequest UserCompany { get; set; }
    public CreateAvatarRequest AvatarImage { get; set; }
    [Required]
    public CreateCommunicationRequest Communication { get; set; }
  }
}