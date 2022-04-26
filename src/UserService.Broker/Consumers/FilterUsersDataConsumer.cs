using LT.DigitalOffice.Kernel.BrokerSupport.Broker;
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
  public class FilterUsersDataConsumer : IConsumer<IFilteredUsersDataRequest>
  {
    private readonly IUserRepository _userRepository;
    private readonly IOptions<RedisConfig> _redisConfig;
    private readonly IGlobalCacheRepository _globalCache;

    private async Task<(List<UserData> userData, int totalCount)> GetUserInfoAsync(IFilteredUsersDataRequest request)
    {
      List<DbUser> dbUsers = new();

      int totalCount = 0;

      if (request.SkipCount > -1 && request.TakeCount > 1)
      {
        (dbUsers, totalCount) = await _userRepository.FindAsync(new FindUsersFilter()
        {
          SkipCount = request.SkipCount,
          TakeCount = request.TakeCount,
          IncludeCurrentAvatar = true,
          IsActive = true,
          IsAscendingSort = request.AscendingSort
        },
        request.UsersIds);
      }

      return (dbUsers.Select(
        u => new UserData(
          u.Id,
          u.Avatars?.FirstOrDefault(ua => ua.IsCurrentAvatar)?.AvatarId,
          u.FirstName,
          u.MiddleName,
          u.LastName,
          ((UserStatus)u.Status).ToString(),
          u.IsActive))
        .ToList(),
        totalCount);
    }

    public FilterUsersDataConsumer(
      IUserRepository userRepository,
      IOptions<RedisConfig> redisConfig,
      IGlobalCacheRepository globalCache)
    {
      _userRepository = userRepository;
      _redisConfig = redisConfig;
      _globalCache = globalCache;
    }

    public async Task Consume(ConsumeContext<IFilteredUsersDataRequest> context)
    {
      (List<UserData> users, int usersCount) = await GetUserInfoAsync(context.Message);

      await context.RespondAsync<IOperationResult<IFilteredUsersDataResponse>>(
        OperationResultWrapper.CreateResponse((_) => IFilteredUsersDataResponse.CreateObj(users, usersCount), context));

      if (users is not null)
      {
        string key = null;

        if (context.Message.AscendingSort.HasValue) 
        {
          key = users.Select(u => u.Id).ToList()
            .GetRedisCacheHashCode(context.Message.SkipCount, context.Message.TakeCount, context.Message.AscendingSort);
        }
        else
        {
          key = users.Select(u => u.Id).ToList()
            .GetRedisCacheHashCode(context.Message.SkipCount, context.Message.TakeCount);
        }
        

        await _globalCache.CreateAsync(
          Cache.Users,
          key,
          (users, usersCount),
          users.Select(u => u.Id).ToList(),
          TimeSpan.FromMinutes(_redisConfig.Value.CacheLiveInMinutes));
      }
    }
  }
}
