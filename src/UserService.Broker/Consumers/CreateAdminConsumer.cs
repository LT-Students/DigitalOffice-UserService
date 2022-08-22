using LT.DigitalOffice.Kernel.BrokerSupport.Broker;
using LT.DigitalOffice.Models.Broker.Requests.User;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using MassTransit;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Broker.Consumers
{
  public class CreateAdminConsumer : IConsumer<ICreateAdminRequest>
  {
    private readonly IUserRepository _userRepository;
    private readonly IUserCredentialsRepository _credentialsRepository;
    private readonly IDbUserMapper _mapper;
    private readonly IDbUserCredentialsMapper _credentialsMapper;

    private async Task<object> CreateAdmin(ICreateAdminRequest request)
    {
      DbUser admin = _mapper.Map(request);
      await _userRepository.CreateAsync(admin);
      await _credentialsRepository
        .CreateAsync(_credentialsMapper.Map(userId: admin.Id, login: request.Login, password: request.Password));

      return true;
    }

    public CreateAdminConsumer(
      IUserRepository userRepository,
      IUserCredentialsRepository credentialsRepository,
      IDbUserMapper mapper,
      IDbUserCredentialsMapper credentialsMapper)
    {
      _userRepository = userRepository;
      _credentialsRepository = credentialsRepository;
      _mapper = mapper;
      _credentialsMapper = credentialsMapper;
    }

    public async Task Consume(ConsumeContext<ICreateAdminRequest> context)
    {
      object response = OperationResultWrapper.CreateResponse(CreateAdmin, context.Message);

      await context.RespondAsync<IOperationResult<bool>>(response);
    }
  }
}
