using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Filtres;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Filters;
using LT.DigitalOffice.UserService.Models.Dto.Responses.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;

namespace LT.DigitalOffice.UserService.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class UsersController : ControllerBase
  {
    private readonly IHttpContextAccessor _httpContextAccessor;
    public UsersController(
      IHttpContextAccessor httpContextAccessor)
    {
      _httpContextAccessor = httpContextAccessor;
    }

    [HttpPost("create")]
    public OperationResultResponse<Guid> Create(
      [FromServices] ICreateUserCommand command,
      [FromBody] CreateUserRequest request)
    {
      var result = command.Execute(request);

      if (result.Status == OperationResultStatusType.Conflict)
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;

        return result;
      }

      if (result.Status != OperationResultStatusType.Failed)
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;
      }

      return result;
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
      [FromQuery] FindUsersFilter filter)
    {
      return command.Execute(filter);
    }
  }
}