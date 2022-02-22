using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Business.Commands.Credentials.Interfaces;
using LT.DigitalOffice.UserService.Business.Commands.Pending.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Credentials;
using LT.DigitalOffice.UserService.Models.Dto.Responses.Credentials;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class CredentialsController : ControllerBase
  {
    [HttpPost("create")]
    public async Task<OperationResultResponse<CredentialsResponse>> CreateCredentials(
      [FromServices] ICreateCredentialsCommand command,
      [FromBody] CreateCredentialsRequest request)
    {
      return await command.ExecuteAsync(request);
    }
  }
}
