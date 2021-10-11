using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Filtres;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Filters;
using LT.DigitalOffice.UserService.Models.Dto.Responses.User;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class UsersController : ControllerBase
  {
    [HttpPost("create")]
    public async Task<OperationResultResponse<Guid>> Create(
      [FromServices] ICreateUserCommand command,
      [FromBody] CreateUserRequest request)
    {
      return await command.Execute(request);
    }

    [HttpPatch("edit")]
    public async Task<OperationResultResponse<bool>> Edit(
      [FromServices] IEditUserCommand command,
      [FromQuery] Guid userId,
      [FromBody] JsonPatchDocument<EditUserRequest> request)
    {
      return await command.Execute(userId, request);
    }

    [HttpGet("get")]
    public async Task<OperationResultResponse<UserResponse>> Get(
      [FromServices] IGetUserCommand command,
      [FromQuery] GetUserFilter filter)
    {
      return await command.Execute(filter);
    }

    [HttpGet("find")]
    public async Task<FindResultResponse<UserInfo>> Find(
      [FromServices] IFindUserCommand command,
      [FromQuery] FindUsersFilter filter)
    {
      return await command.Execute(filter);
    }
  }
}