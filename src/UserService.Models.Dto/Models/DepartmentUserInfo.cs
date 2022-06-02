using LT.DigitalOffice.Models.Broker.Enums;

namespace LT.DigitalOffice.UserService.Models.Dto.Models
{
  public record DepartmentUserInfo
  {
    public DepartmentInfo Department { get; set; }
    public DepartmentUserRole Role { get; set; }
  }
}
