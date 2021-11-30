using LT.DigitalOffice.Kernel.BrokerSupport.Broker;
using LT.DigitalOffice.Models.Broker.Common;
using LT.DigitalOffice.UserService.Data.Interfaces;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Broker.Consumers
{
  public class CheckUsersExistenceConsumer : IConsumer<ICheckUsersExistence>
  {
    private readonly IUserRepository _repository;

    public CheckUsersExistenceConsumer(IUserRepository repository)
    {
      _repository = repository;
    }

    public async Task Consume(ConsumeContext<ICheckUsersExistence> context)
    {
      var response = OperationResultWrapper.CreateResponse(GetUsersExistenceInfoAsync, context.Message);

      await context.RespondAsync<IOperationResult<ICheckUsersExistence>>(response);
    }

    public async Task<object> GetUsersExistenceInfoAsync(ICheckUsersExistence requestIds)
    {
      List<Guid> userIds = await _repository.AreExistingIdsAsync(requestIds.UserIds);

      return ICheckUsersExistence.CreateObj(userIds);
    }
  }
}
