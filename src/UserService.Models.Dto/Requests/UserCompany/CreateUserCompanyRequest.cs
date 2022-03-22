using System;

namespace LT.DigitalOffice.UserService.Models.Dto.Requests.UserCompany
{
  public record CreateUserCompanyRequest
  {
    public Guid CompanyId { get; set; }
    public double? Rate { get; set; }
    public DateTime? StartWorkingAt { get; set; }
  }
}
