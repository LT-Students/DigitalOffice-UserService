﻿using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Models.Company;
using LT.DigitalOffice.UserService.Models.Dto.Models;

namespace LT.DigitalOffice.UserService.Mappers.Models.Interfaces
{
  [AutoInject]
  public interface ICompanyUserInfoMapper
  {
    CompanyUserInfo Map(CompanyData companyData, CompanyUserData companyUserData);
  }
}
