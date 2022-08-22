using LT.DigitalOffice.Models.Broker.Models.Company;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Models;

namespace LT.DigitalOffice.UserService.Mappers.Models
{
  public class CompanyUserInfoMapper : ICompanyUserInfoMapper
  {
    public CompanyUserInfo Map(CompanyData companyData, CompanyUserData companyUserData)
    {
      return companyData is null || companyUserData is null
        ? default
        : new CompanyUserInfo
        {
          Company = new CompanyInfo
          {
            Id = companyData.Id,
            Name = companyData.Name
          },
          ContractSubject = companyUserData.ContractSubject,
          ContractTermType = companyUserData.ContractTermType,
          Rate = companyUserData.Rate,
          StartWorkingAt = companyUserData.StartWorkingAt,
          EndWorkingAt = companyUserData.EndWorkingAt,
          Probation = companyUserData.Probation
        };
    }
  }
}
