using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Models.Broker.Requests.User;
using LT.DigitalOffice.UserService.Business.Helpers.Password;
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
        private readonly IUserRepository _repository;
        private readonly IDbUserMapper _mapper;

        private object CreateAdmin(ICreateAdminRequest request)
        {
            string salt = $"{Guid.NewGuid()}{Guid.NewGuid()}";

            DbUser admin = _mapper.Map(request);

            DbUserCredentials adminCredentials = new()
            {
                Id = Guid.NewGuid(),
                UserId = admin.Id,
                Login = request.Login,
                Salt = salt,
                PasswordHash = UserPasswordHash.GetPasswordHash(request.Login, salt, request.Password)
            };

            _repository
                .Create(admin, adminCredentials);

            return true;
        }

        public CreateAdminConsumer(
            IUserRepository repository,
            IDbUserMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task Consume(ConsumeContext<ICreateAdminRequest> context)
        {
            var response = OperationResultWrapper.CreateResponse(CreateAdmin, context.Message);

            await context.RespondAsync<IOperationResult<bool>>(response);
        }
    }
}
