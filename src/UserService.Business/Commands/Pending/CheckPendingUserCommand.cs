using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Business.Commands.Pending.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.Credentials
{
  public class CheckPendingUserCommand : ICheckPendingUserCommand
  {
    private readonly IPendingUserRepository _repository;

    public CheckPendingUserCommand(IPendingUserRepository repository)
    {
      _repository = repository;
    }

    public async Task<OperationResultResponse<bool>> ExecuteAsync(Guid userId)
    {
      return new OperationResultResponse<bool>(
        body: await _repository.DoesExistAsync(userId));
    }
  }
}
