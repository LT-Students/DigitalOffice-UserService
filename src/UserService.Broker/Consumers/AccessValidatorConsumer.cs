using LT.DigitalOffice.Kernel.AccessValidatorEngine.Requests;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.UserService.Data.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Broker.Consumers
{
  public class AccessValidatorConsumer : IConsumer<ICheckUserIsAdminRequest>
  {
    private readonly IUserRepository repository;

    public AccessValidatorConsumer([FromServices] IUserRepository repository)
    {
      this.repository = repository;
    }

    public async Task Consume(ConsumeContext<ICheckUserIsAdminRequest> context)
    {
      var response = OperationResultWrapper.CreateResponse(IsAdminAsync, context.Message.UserId);

      await context.RespondAsync<IOperationResult<bool>>(response);
    }

    private async Task<object> IsAdminAsync(Guid userId)
    {
      return (await repository.GetAsync(userId)).IsAdmin;
    }
  }
}
