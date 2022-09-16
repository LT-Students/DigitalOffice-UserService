using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.UserService.Business.Commands.User.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.User
{
  public class GetUserInfoCommand : IGetUserInfoCommand
  {
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRepository _repository;

    public GetUserInfoCommand(
      IHttpContextAccessor httpContextAccessor,
      IUserRepository repository)
    {
      _httpContextAccessor = httpContextAccessor;
      _repository = repository;
    }

    public async Task<OperationResultResponse<UserData>> ExecuteAsync()
    {
      DbUser dbUser = await _repository.GetAsync(_httpContextAccessor.HttpContext.GetUserId());
      return new OperationResultResponse<UserData>(body: new UserData(
          id: dbUser.Id,
          imageId: null,
          firstName: dbUser.FirstName,
          middleName: dbUser.MiddleName,
          lastName: dbUser.LastName,
          isActive: dbUser.IsActive,
          email: null));
    }
  }
}
