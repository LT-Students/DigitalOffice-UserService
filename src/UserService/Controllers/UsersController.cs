using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Filters;
using LT.DigitalOffice.UserService.Models.Dto.Responses.User;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;

namespace LT.DigitalOffice.UserService.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class UsersController : ControllerBase
  {
    [HttpPost("create")]
    public OperationResultResponse<Guid> Create(
      [FromServices] ICreateUserCommand command,
      [FromBody] CreateUserRequest request)
    {
      return command.Execute(request);
    }

    [HttpPatch("edit")]
    public OperationResultResponse<bool> Edit(
      [FromServices] IEditUserCommand command,
      [FromQuery] Guid userId,
      [FromBody] JsonPatchDocument<EditUserRequest> request)
    {
      return command.Execute(userId, request);
    }

    [HttpGet("get")]
    public OperationResultResponse<UserResponse> Get(
      [FromServices] IGetUserCommand command,
      [FromQuery] GetUserFilter filter)
    {
      return command.Execute(filter);
    }

    [HttpGet("find")]
    public FindResultResponse<UserInfo> Find(
      [FromServices] IFindUserCommand command,
      [FromQuery] int skipCount,
      [FromQuery] int takeCount,
      [FromQuery] Guid? departmentId)
    {
      return command.Execute(skipCount, takeCount, departmentId);
    }
  }
}