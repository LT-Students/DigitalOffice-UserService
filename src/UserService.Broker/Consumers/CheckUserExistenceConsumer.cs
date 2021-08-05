using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Models.Broker.Common;
using LT.DigitalOffice.UserService.Data.Interfaces;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Broker.Consumers
{
    public class CheckUserExistenceConsumer : IConsumer<ICheckUserExistence>
    {
        private readonly IUserRepository _repository;

        public CheckUserExistenceConsumer(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task Consume(ConsumeContext<ICheckUserExistence> context)
        {
            var response = OperationResultWrapper.CreateResponse(GetUserExistenceInfo, context.Message);

            await context.RespondAsync<IOperationResult<ICheckUserExistence>>(response);
        }

        private object GetUserExistenceInfo(ICheckUserExistence requestIds)
        {
            List<Guid> userIds = _repository.AreExistingIds(requestIds.UserIds);

            return ICheckUserExistence.CreateObj(userIds);
        }
    }
}
