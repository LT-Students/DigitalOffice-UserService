using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController([FromServices] IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("getUserById")]
        public User GetUserById([FromServices] IGetUserByIdCommand getUserInfoByIdCommand, [FromQuery] Guid userId)
        {
            return getUserInfoByIdCommand.Execute(userId);
        }

        [HttpPost("createUser")]
        public Guid CreateUser([FromServices] ICreateUserCommand command, [FromBody] UserRequest request)
        {
            return command.Execute(request);
        }

        [HttpPost("editUser")]
        public bool EditUser([FromServices] IEditUserCommand command, [FromBody] UserRequest request)
        {
            return command.Execute(request);
        }

        [HttpGet("generatePassword")]
        public string GeneratePassword([FromServices] IGeneratePasswordCommand command)
        {
            return command.Execute();
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

        [HttpGet("getUsersByIds")]
        public IEnumerable<User> GetUsersByIds(
            [FromServices] IGetUsersByIdsCommand command,
            [FromQuery] IEnumerable<Guid> usersIds)
        {
            return command.Execute(usersIds);
        }

        [HttpDelete("disableUserById")]
        public void DisableUserById(
            [FromServices] IDisableUserByIdCommand command,
            [FromQuery] Guid userId)
        {
            command.Execute(userId);
        }

        [HttpGet("forgotPassword")]
        public void ForgotPassword([FromServices] IForgotPasswordCommand command,
            [FromQuery] string userEmail)
        {
            command.Execute(userEmail);
        }

        [HttpGet("getAllUsers")]
        public IEnumerable<User> GetAllUsers(
            [FromServices] IGetAllUsersCommand command,
            [FromQuery] int skipCount,
            [FromQuery] int takeCount,
            [FromQuery] string userNameFilter)
        {
            return command.Execute(skipCount, takeCount, userNameFilter);
        }
    }
}