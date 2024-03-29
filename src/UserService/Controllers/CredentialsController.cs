﻿using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Business.Commands.Credentials.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Credentials;
using LT.DigitalOffice.UserService.Models.Dto.Responses.Credentials;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class CredentialsController : ControllerBase
  {
    [HttpPost("create")]
    public async Task<OperationResultResponse<CredentialsResponse>> CreateAsync(
      [FromServices] ICreateCredentialsCommand command,
      [FromBody] CreateCredentialsRequest request)
    {
      return await command.ExecuteAsync(request);
    }

    [HttpPut("reactivate")]
    public async Task<OperationResultResponse<CredentialsResponse>> ReactivateAcync(
      [FromServices] IReactivateCredentialsCommand command,
      [FromBody] ReactivateCredentialsRequest request)
    {
      return await command.ExecuteAsync(request);
    }
  }
}
