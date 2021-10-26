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
    private readonly IUserRepository _userRepository;
    private readonly IImageRepository _imageRepository;
    private readonly IConnectionMultiplexer _cache;
    private readonly IOptions<RedisConfig> _redisConfig;
    private readonly IRedisHelper _redisHelper;
    private readonly ICacheNotebook _cacheNotebook;

    private async Task<List<UserData>> GetUserInfoAsync(IGetUsersDataRequest request)
    {
      List<(DbUser user, Guid avatarId)> users = await _userRepository.GetWithAvatarsAsync(request.UserIds);

      return users
        .Select(users => new UserData(
          users.user.Id,
          users.avatarId == default ? null : users.avatarId,
          users.user.FirstName,
          users.user.MiddleName,
          users.user.LastName,
          ((UserStatus)users.user.Status).ToString(),
          (float)users.user.Rate,
          users.user.IsActive))
        .ToList();
    }

    public GetUsersDataConsumer(
      IUserRepository userRepository,
      IImageRepository imageRepository,
      IConnectionMultiplexer cache,
      IOptions<RedisConfig> redisConfig,
      IRedisHelper redisHelper,
      ICacheNotebook cacheNotebook)
    {
      _userRepository = userRepository;
      _imageRepository = imageRepository;
      _cache = cache;
      _redisConfig = redisConfig;
      _redisHelper = redisHelper;
      _cacheNotebook = cacheNotebook;
    }

    public async Task Consume(ConsumeContext<IGetUsersDataRequest> context)
    {
      List<UserData> users = await GetUserInfoAsync(context.Message);

      await context.RespondAsync<IOperationResult<IGetUsersDataResponse>>(
        OperationResultWrapper.CreateResponse((_) => IGetUsersDataResponse.CreateObj(users), context));

      if (users != null)
      {
        string key = context.Message.UserIds.GetRedisCacheHashCode();

        await _redisHelper.CreateAsync(
          Cache.Users,
          key,
          users,
          TimeSpan.FromMinutes(_redisConfig.Value.CacheLiveInMinutes));

        _cacheNotebook.Add(context.Message.UserIds, Cache.Users, key);
      }
    }
  }
}