﻿using LT.DigitalOffice.Kernel.BrokerSupport.Broker;
using LT.DigitalOffice.Kernel.RedisSupport.Configurations;
using LT.DigitalOffice.Kernel.RedisSupport.Constants;
using LT.DigitalOffice.Kernel.RedisSupport.Extensions;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Requests.User;
using LT.DigitalOffice.Models.Broker.Responses.User;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Filtres;
using MassTransit;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Broker.Consumers
{
  public class GetUsersDataConsumer : IConsumer<IGetUsersDataRequest>
  {
    private readonly IUserRepository _userRepository;
    private readonly IOptions<RedisConfig> _redisConfig;
    private readonly IGlobalCacheRepository _globalCache;

    private async Task<List<UserData>> GetUserInfoAsync(IGetUsersDataRequest request)
    {
      (List<DbUser> dbUsers, int totalCount) =
        await _userRepository.FindAsync(
          filter: new FindUsersFilter() { TakeCount = int.MaxValue, IncludeCurrentAvatar = true, IncludeCommunications = request.IncludeBaseEmail }, //TODO fix takeCount
          userIds: request.UsersIds);

      return dbUsers.Select(
        u => new UserData(
          id: u.Id,
          imageId: u.Avatars?.FirstOrDefault()?.AvatarId,
          firstName: u.FirstName,
          middleName: u.MiddleName,
          lastName: u.LastName,
          isActive: u.IsActive,
          email: request.IncludeBaseEmail
            ? u.Communications.FirstOrDefault(c => c.Type == (int)CommunicationType.BaseEmail)?.Value
            : null))
        .ToList();
    }

    public GetUsersDataConsumer(
      IUserRepository userRepository,
      IOptions<RedisConfig> redisConfig,
      IGlobalCacheRepository globalCache)
    {
      _userRepository = userRepository;
      _redisConfig = redisConfig;
      _globalCache = globalCache;
    }

    public async Task Consume(ConsumeContext<IGetUsersDataRequest> context)
    {
      List<UserData> users = await GetUserInfoAsync(context.Message);

      await context.RespondAsync<IOperationResult<IGetUsersDataResponse>>(
        OperationResultWrapper.CreateResponse((_) => IGetUsersDataResponse.CreateObj(users), context));

      if (users is not null)
      {
        await _globalCache.CreateAsync(
          Cache.Users,
          users.Select(u => u.Id).GetRedisCacheKey(nameof(IGetUsersDataRequest), context.Message.GetBasicProperties()),
          users,
          users.Select(u => u.Id).ToList(),
          TimeSpan.FromMinutes(_redisConfig.Value.CacheLiveInMinutes));
      }
    }
  }
}