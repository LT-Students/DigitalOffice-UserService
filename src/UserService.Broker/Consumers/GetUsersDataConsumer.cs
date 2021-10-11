using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Requests.User;
using LT.DigitalOffice.Models.Broker.Responses.User;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Configurations;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using MassTransit;
using Microsoft.Extensions.Options;
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
    private readonly IRedisHelper _redisHelper;

    private List<UserData> GetUserInfo(IGetUsersDataRequest request)
    {
      List<DbUser> dbUsers = _repository.Get(request.UserIds);

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
      IOptions<RedisConfig> redisConfig,
      IRedisHelper redisHelper)
    {
      _repository = repository;
      _cache = cache;
      _redisConfig = redisConfig;
      _redisHelper = redisHelper;
    }

    public async Task Consume(ConsumeContext<IGetUsersDataRequest> context)
    {
      List<UserData> users = GetUserInfo(context.Message);

      await context.RespondAsync<IOperationResult<IGetUsersDataResponse>>(
        OperationResultWrapper.CreateResponse((_) => IGetUsersDataResponse.CreateObj(users), context));

      if (users != null)
      {
        await _redisHelper.CreateAsync(
          Cache.Users,
          context.Message.UserIds.GetRedisCacheHashCode(),
          users,
          TimeSpan.FromMinutes(_redisConfig.Value.CacheLiveInMinutes));
      }
    }
  }
}