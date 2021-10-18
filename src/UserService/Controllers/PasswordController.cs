﻿using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Business.Commands.Password.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class PasswordController : ControllerBase
  {
    [HttpGet("forgot")]
    public async Task<OperationResultResponse<bool>> ForgotPasswordAsync(
      [FromServices] IForgotPasswordCommand command,
      [FromQuery] string userEmail)
    {
      return await command.ExecuteAsync(userEmail);
    }

    [HttpPost("change")]
    public async Task<OperationResultResponse<bool>> ChangePasswordAsync(
      [FromServices] IChangePasswordCommand command,
      [FromBody] ChangePasswordRequest request)
    {
      return await command.ExecuteAsync(request);
    }

    [HttpGet("generate")]
    public string GeneratePassword(
      [FromServices] IGeneratePasswordCommand command)
    {
      return command.Execute();
    }
  }
}
