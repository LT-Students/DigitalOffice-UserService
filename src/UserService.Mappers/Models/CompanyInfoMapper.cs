using LT.DigitalOffice.Models.Broker.Models.Company;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Models;

namespace LT.DigitalOffice.UserService.Mappers.Models
{
  public class CompanyInfoMapper : ICompanyInfoMapper
  {
    public CompanyInfo Map(CompanyData companyData)
    {
      if (companyData is null)
      {
        return null;
      }

      return new CompanyInfo
      {
        Id = companyData.Id,
        Name = companyData.Name
      };
    }
  }
}
