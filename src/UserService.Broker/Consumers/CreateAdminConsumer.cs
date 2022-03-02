﻿using LT.DigitalOffice.Kernel.BrokerSupport.Broker;
using LT.DigitalOffice.Models.Broker.Requests.User;
using LT.DigitalOffice.UserService.Broker.Helpers.Password;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using MassTransit;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Broker.Consumers
{
  public class CreateAdminConsumer : IConsumer<ICreateAdminRequest>
  {
    private readonly IUserRepository _userRepository;
    private readonly IUserCredentialsRepository _credentialsRepository;
    private readonly IDbUserMapper _mapper;

    private async Task<object> CreateAdmin(ICreateAdminRequest request)
    {
      string salt = $"{Guid.NewGuid()}{Guid.NewGuid()}";

      DbUser admin = _mapper.Map(request);
      await _userRepository.CreateAsync(admin);

      DbUserCredentials adminCredentials = new()
      {
        Id = Guid.NewGuid(),
        UserId = admin.Id,
        Login = request.Login,
        Salt = salt,
        PasswordHash = UserPasswordHash.GetPasswordHash(request.Login, salt, request.Password),
        IsActive = true
      };
      await _credentialsRepository.CreateAsync(adminCredentials);

      return true;
    }

    public CreateAdminConsumer(
      IUserRepository userRepository,
      IUserCredentialsRepository credentialsRepository,
      IDbUserMapper mapper)
    {
      _userRepository = userRepository;
      _credentialsRepository = credentialsRepository;
      _mapper = mapper;
    }

    public async Task Consume(ConsumeContext<ICreateAdminRequest> context)
    {
      var response = OperationResultWrapper.CreateResponse(CreateAdmin, context.Message);

      await context.RespondAsync<IOperationResult<bool>>(response);
    }
  }
}
