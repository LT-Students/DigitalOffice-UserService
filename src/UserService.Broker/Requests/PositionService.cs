using LT.DigitalOffice.Kernel.BrokerSupport.Helpers;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.RedisSupport.Constants;
using LT.DigitalOffice.Kernel.RedisSupport.Extensions;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Models.Broker.Models.Position;
using LT.DigitalOffice.Models.Broker.Requests.Position;
using LT.DigitalOffice.Models.Broker.Responses.Position;
using LT.DigitalOffice.UserService.Broker.Requests.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Broker.Requests
{
  public class PositionService : IPositionService
  {
    private readonly IRequestClient<ICreateUserPositionRequest> _rcCreateUserPosition;
    private readonly IRequestClient<IGetPositionsRequest> _rcGetPositions;
    private readonly ILogger<PositionService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IGlobalCacheRepository _globalCache;

    public PositionService(
      IRequestClient<ICreateUserPositionRequest> rcCreateUserPosition,
      IRequestClient<IGetPositionsRequest> rcGetPositions,
      ILogger<PositionService> logger,
      IHttpContextAccessor httpContextAccessor,
      IGlobalCacheRepository globalCache)
    {
      _rcCreateUserPosition = rcCreateUserPosition;
      _rcGetPositions = rcGetPositions;
      _logger = logger;
      _httpContextAccessor = httpContextAccessor;
      _globalCache = globalCache;
    }

    public async Task CreateUserPositionAsync(Guid positionId, Guid userId, List<string> errors)
    {
      if (positionId != Guid.Empty)
      {
        await RequestHandler.ProcessRequest<ICreateUserPositionRequest, bool>(
          _rcCreateUserPosition,
          ICreateUserPositionRequest.CreateObj(
            positionId: positionId,
            createdBy: _httpContextAccessor.HttpContext.GetUserId(),
            userId: userId),
          errors,
          _logger);
      }
    }

    public async Task<List<PositionData>> GetPositionsAsync(
      Guid userId,
      List<string> errors)
    {
      List<PositionData> positions = await _globalCache
        .GetAsync<List<PositionData>>(Cache.Positions, userId.GetRedisCacheHashCode());

      if (positions is not null)
      {
        _logger.LogInformation(
          "Positions for user id '{UserId}' were taken from cache.",
          userId);
      }
      else
      {
        positions = (await RequestHandler.ProcessRequest<IGetPositionsRequest, IGetPositionsResponse>(
            _rcGetPositions,
            IGetPositionsRequest.CreateObj(usersIds: new() { userId }),
            errors,
            _logger))
          ?.Positions;
      }

      return positions;
    }
  }
}
