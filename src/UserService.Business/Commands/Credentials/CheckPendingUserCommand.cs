using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Business.Commands.Credentials.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.Credentials
{
  public class CheckPendingUserCommand : ICheckPendingUserCommand
  {
    private readonly IUserRepository _repository;

    public CheckPendingUserCommand(IUserRepository repository)
    {
      _repository = repository;
    }

    public async Task<OperationResultResponse<bool>> ExecuteAsync(Guid pendingUserId)
    {
      return new()
      {
        Body = await _repository.PendingUserExistAsync(pendingUserId),
        Status = Kernel.Enums.OperationResultStatusType.FullSuccess
      };
    }
  }
}
