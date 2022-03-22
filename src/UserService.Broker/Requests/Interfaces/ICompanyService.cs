using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Models.Company;
using LT.DigitalOffice.UserService.Models.Dto.Requests.UserCompany;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Broker.Requests.Interfaces
{
  [AutoInject]
  public interface ICompanyService
  {
    Task CreateUserCompanyAsync(CreateUserCompanyRequest request, Guid userId, List<string> errors);

    Task<List<CompanyData>> GetCompaniesAsync(Guid userId, List<string> errors);
  }
}
