using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Requests.User;
using LT.DigitalOffice.Models.Broker.Responses.User;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Configurations;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using MassTransit;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Broker.Consumers
{
  /// <summary>
  /// Consumer for getting information about the users.
  /// </summary>
  public class GetUsersDataConsumer : IConsumer<IGetUsersDataRequest>
  {
    private readonly IUserRepository _repository;
    private readonly IConnectionMultiplexer _cache;
    private readonly IOptions<RedisConfig> _redisConfig;

    private List<UserData> GetUserInfo(IGetUsersDataRequest request)
    {
      var dbUsers = _repository.Get(request.UserIds);

      return dbUsers
        .Select(dbUser => new UserData(
          dbUser.Id,
          dbUser.AvatarFileId,
          dbUser.FirstName,
          dbUser.MiddleName,
          dbUser.LastName,
          ((UserStatus)dbUser.Status).ToString(),
          (float)dbUser.Rate,
          dbUser.IsActive))
        .ToList();
    }

    public GetUsersDataConsumer(
      IUserRepository repository,
      IConnectionMultiplexer cache,
      IOptions<RedisConfig> redisConfig)
    {
      _repository = repository;
      _cache = cache;
      _redisConfig = redisConfig;
    }

    public async Task Consume(ConsumeContext<IGetUsersDataRequest> context)
    {
      List<UserData> users = GetUserInfo(context.Message);

      await context.RespondAsync<IOperationResult<IGetUsersDataResponse>>(
        OperationResultWrapper.CreateResponse((_) => users, context));

      await _cache.GetDatabase(Cache.Users).StringSetAsync(
        users.Select(u => u.Id).GetRedisCacheKey(), 
        JsonConvert.SerializeObject(users), 
        TimeSpan.FromMinutes(_redisConfig.Value.CacheLiveInMinutes));
    }
  }
}