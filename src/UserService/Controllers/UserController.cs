﻿using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

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
        public Guid CreateUser([FromServices] ICreateUserCommand command, [FromBody] UserRequest request)
        {
            return command.Execute(request);
        }

        [HttpPost("editUser")]
        public bool EditUser([FromServices] IEditUserCommand command, [FromBody] UserRequest request)
        {
            return command.Execute(request);
        }

        [HttpGet("getUserByEmail")]
        public User GetUserByEmail([FromServices] IGetUserByEmailCommand command, [FromQuery] string userEmail)
        {
            return command.Execute(userEmail);
        }

        [HttpGet("getUsersByIds")]
        public List<User> GetUsersByIds(
            [FromServices] IGetUsersByIdsCommand command,
            [FromQuery] IEnumerable<Guid> usersIds)
        {
            return command.Execute(usersIds);
        }

        [HttpDelete("disableUserById")]
        public void DisableUserById(
            [FromServices] IDisableUserByIdCommand command,
            [FromQuery] Guid userId,
            [FromHeader] Guid requestingUser)
        {
            command.Execute(userId, requestingUser);
        }

        [HttpGet("forgotPassword")]
        public void ForgotPassword([FromServices] IForgotPasswordCommand command,
            [FromQuery] string userEmail)
        {
            command.Execute(userEmail);
        }
    }
}