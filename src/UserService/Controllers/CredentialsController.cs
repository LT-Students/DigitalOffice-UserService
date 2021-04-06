using LT.DigitalOffice.UserService.Business.Commands.Credentials.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Credentials;
using LT.DigitalOffice.UserService.Models.Dto.Responses.Credentials;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.UserService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CredentialsController : ControllerBase
    {
        [HttpPost("create")]
        public CredentialsResponse CreateCredentials(
            [FromServices] ICreateCredentialsCommand command,
            [FromBody] CreateCredentialsRequest request)
        {
            return command.Execute(request);
        }
    }
}
