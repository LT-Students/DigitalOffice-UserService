using LT.DigitalOffice.Kernel.BrokerSupport.Helpers;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.RedisSupport.Constants;
using LT.DigitalOffice.Kernel.RedisSupport.Extensions;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Models.Broker.Models.Company;
using LT.DigitalOffice.Models.Broker.Requests.Company;
using LT.DigitalOffice.Models.Broker.Responses.Company;
using LT.DigitalOffice.UserService.Broker.Requests.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Requests.UserCompany;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Broker.Requests
{
  public class CompanyService : ICompanyService
  {
    private readonly IRequestClient<ICreateCompanyUserRequest> _rcCreateCompanyUser;
    private readonly IRequestClient<IGetCompaniesRequest> _rcGetCompanies;
    private readonly ILogger<CompanyService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IGlobalCacheRepository _globalCache;

    public CompanyService(
      IRequestClient<ICreateCompanyUserRequest> rcCreateCompanyUser,
      ILogger<CompanyService> logger,
      IHttpContextAccessor httpContextAccessor,
      IGlobalCacheRepository globalCache)
    {
      _rcCreateCompanyUser = rcCreateCompanyUser;
      _logger = logger;
      _httpContextAccessor = httpContextAccessor;
      _globalCache = globalCache;
    }

    public async Task CreateUserCompanyAsync(CreateUserCompanyRequest request, Guid userId, List<string> errors)
    {
      if (request is not null)
      {
        await RequestHandler.ProcessRequest<ICreateCompanyUserRequest, bool>(
          _rcCreateCompanyUser,
          ICreateCompanyUserRequest.CreateObj(
            companyId: request.CompanyId,
            userId: userId,
            rate: request.Rate,
            startWorkingAt: request.StartWorkingAt,
            createdBy: _httpContextAccessor.HttpContext.GetUserId()),
          errors,
          _logger);
      }
    }

    public async Task<List<CompanyData>> GetCompaniesAsync(
      Guid userId,
      List<string> errors)
    {
      List<CompanyData> companies = await _globalCache
        .GetAsync<List<CompanyData>>(Cache.Companies, userId.GetRedisCacheHashCode());

      if (companies is not null)
      {
        _logger.LogInformation(
          "Companies for user id '{UserId}' were taken from cache.",
          userId);
      }
      else
      {
        companies = (await RequestHandler.ProcessRequest<IGetCompaniesRequest, IGetCompaniesResponse>(
            _rcGetCompanies,
            IGetCompaniesRequest.CreateObj(usersIds: new() { userId }),
            errors,
            _logger))
          ?.Companies;
      }

      return companies;
    }
  }
}
