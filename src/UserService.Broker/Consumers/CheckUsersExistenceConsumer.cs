using LT.DigitalOffice.Kernel.Broker;
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
            var response = OperationResultWrapper.CreateResponse(GetUsersExistenceInfo, context.Message);

            await context.RespondAsync<IOperationResult<ICheckUsersExistence>>(response);
        }

        private object GetUsersExistenceInfo(ICheckUsersExistence requestIds)
        {
            List<Guid> userIds = _repository.AreExistingIds(requestIds.UserIds);

            return ICheckUsersExistence.CreateObj(userIds);
        }
    }
}
