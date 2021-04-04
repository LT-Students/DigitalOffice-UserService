using LT.DigitalOffice.UserService.Business.Commands.Password.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto;
using LT.DigitalOffice.UserService.Models.Dto.Responses;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.UserService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PasswordController : ControllerBase
    {
        [HttpGet("forgot")]
        public OperationResultResponse<bool> ForgotPassword(
            [FromServices] IForgotPasswordCommand command,
            [FromQuery] string userEmail)
        {
            return command.Execute(userEmail);
        }

        [HttpPost("change")]
        public OperationResultResponse<bool> ChangePassword(
            [FromServices] IChangePasswordCommand command,
            [FromBody] ChangePasswordRequest request)
        {
            return command.Execute(request);
        }

        [HttpGet("generate")]
        public string GeneratePassword(
            [FromServices] IGeneratePasswordCommand command)
        {
            return command.Execute();
        }
    }
}
