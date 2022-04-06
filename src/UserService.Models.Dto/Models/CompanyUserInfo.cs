using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Models.Company;
using System;

namespace LT.DigitalOffice.UserService.Models.Dto.Models
{
  public record CompanyUserInfo
  {
    public CompanyInfo Company { get; set; }
    public ContractSubjectData ContractSubject { get; set; }
    public ContractTerm ContratcTermType { get; set; }
    public double? Rate { get; set; }
    public DateTime StartWorkingAt { get; set; }
    public DateTime? EndWorkingAt { get; set; }
    public DateTime? Probation { get; set; }
  }
}
