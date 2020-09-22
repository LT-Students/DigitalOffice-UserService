using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpGet("getUserById")]
        public User GetUserById([FromServices] IGetUserByIdCommand getUserInfoByIdCommand, [FromQuery] Guid userId)
            => getUserInfoByIdCommand.Execute(userId);

        [HttpPost("register")]
        public Guid CreateUser([FromServices] IUserCreateCommand command, [FromBody] UserCreateRequest request)
        {
            return command.Execute(request);
        }

        [HttpPost("editUser")]
        public bool EditUser([FromServices] IEditUserCommand command, [FromBody] EditUserRequest request)
        {
            return command.Execute(request);
        }

        [HttpPost("changePassword")]
        public void ChangePassword([FromServices] IChangePasswordCommand command, [FromBody] ChangePasswordRequest request)
        {
            command.Execute(request);
        }

        [HttpGet("getUserByEmail")]
        public User GetUserByEmail([FromServices] IGetUserByEmailCommand command, [FromQuery] string userEmail)
        {
            return command.Execute(userEmail);
        }

        [HttpDelete("disableUserById")]
        public void DisableUserById(
            [FromServices] IDisableUserByIdCommand command,
            [FromQuery] Guid userId,
            [FromHeader] Guid requestingUser)
        {
            command.Execute(userId, requestingUser);
        }
    }
}