using LT.DigitalOffice.Kernel.BrokerSupport.Helpers;
using LT.DigitalOffice.Kernel.RedisSupport.Constants;
using LT.DigitalOffice.Kernel.RedisSupport.Extensions;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Models.Broker.Models.Office;
using LT.DigitalOffice.Models.Broker.Requests.Office;
using LT.DigitalOffice.Models.Broker.Responses.Office;
using LT.DigitalOffice.UserService.Broker.Requests.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Broker.Requests
{
  public class OfficeService : IOfficeService
  {
    private readonly IRequestClient<IGetOfficesRequest> _rcGetOffices;
    private readonly ILogger<OfficeService> _logger;
    private readonly IGlobalCacheRepository _globalCache;

    public OfficeService(
      IRequestClient<IGetOfficesRequest> rcGetOffices,
      ILogger<OfficeService> logger,
      IGlobalCacheRepository globalCache)
    {
      _rcGetOffices = rcGetOffices;
      _logger = logger;
      _globalCache = globalCache;
    }

    public async Task<List<OfficeData>> GetOfficesAsync(
      Guid userId,
      List<string> errors,
      CancellationToken token)
    {
      /*List<OfficeData> offices = await _globalCache
        .GetAsync<List<OfficeData>>(Cache.Offices, userId.GetRedisCacheHashCode());

      if (offices is not null)
      {
        _logger.LogInformation(
          "Offices for user id '{UserId}' were taken from cache.",
          userId);
      }
      else
      {
        offices = (await RequestHandler.ProcessRequest<IGetOfficesRequest, IGetOfficesResponse>(
            _rcGetOffices,
            IGetOfficesRequest.CreateObj(usersIds: new() { userId }),
            errors,
            _logger))
          ?.Offices;
      }

      return offices;*/
      return null;
    }
  }
}
