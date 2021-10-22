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
    private readonly IUserRepository _userRepository;
    private readonly IImageRepository _imageRepository;
    private readonly IConnectionMultiplexer _cache;
    private readonly IOptions<RedisConfig> _redisConfig;
    private readonly IRedisHelper _redisHelper;

    private async Task<List<UserData>> GetUserInfoAsync(IGetUsersDataRequest request)
    {
      List<DbUser> dbUsers = await _userRepository.GetAsync(request.UserIds);
      List<DbEntityImage> usersAvatars = await _imageRepository.GetAvatarsAsync(request.UserIds);

      return dbUsers
        .Select(dbUser => new UserData(
          dbUser.Id,
          usersAvatars.Where(x => x.EntityId == dbUser.Id).Select(x => x.ImageId).FirstOrDefault(),
          dbUser.FirstName,
          dbUser.MiddleName,
          dbUser.LastName,
          ((UserStatus)dbUser.Status).ToString(),
          (float)dbUser.Rate,
          dbUser.IsActive))
        .ToList();
    }

    public GetUsersDataConsumer(
      IUserRepository userRepository,
      IImageRepository imageRepository,
      IConnectionMultiplexer cache,
      IOptions<RedisConfig> redisConfig,
      IRedisHelper redisHelper)
    {
      _userRepository = userRepository;
      _imageRepository = imageRepository;
      _cache = cache;
      _redisConfig = redisConfig;
      _redisHelper = redisHelper;
    }

    public async Task Consume(ConsumeContext<IGetUsersDataRequest> context)
    {
      List<UserData> users = await GetUserInfoAsync(context.Message);

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