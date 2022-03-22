using LT.DigitalOffice.Models.Broker.Models.Company;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using System.Linq;

namespace LT.DigitalOffice.UserService.Mappers.Models
{
  public class CompanyUserInfoMapper : ICompanyUserInfoMapper
  {
    public CompanyUserInfo Map(CompanyData companyData, CompanyUserData companyUserData)
    {
      if (companyData is null || companyUserData is null)
      {
        return null;
      }

      return new CompanyUserInfo
      {
        Company = new CompanyInfo
        {
          Id = companyData.Id,
          Name = companyData.Name
        },
        Rate = companyUserData.Rate,
        StartWorkingAt = companyUserData.StartWorkingAt
      };
    }
  }
}
