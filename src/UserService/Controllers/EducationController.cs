﻿using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Business.Commands.Education.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Education;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EducationController : ControllerBase
    {
        [HttpPost("create")]
        public async Task<OperationResultResponse<Guid>> Create(
            [FromServices] ICreateEducationCommand command,
            [FromBody] CreateEducationRequest request)
        {
            return await command.Execute(request);
        }

        [HttpPatch("edit")]
        public OperationResultResponse<bool> Edit(
            [FromServices] IEditEducationCommand command,
            [FromQuery] Guid educationId,
            [FromBody] JsonPatchDocument<EditEducationRequest> request)
        {
            return command.Execute(educationId, request);
        }

        [HttpDelete("remove")]
        public async Task<OperationResultResponse<bool>> Remove(
            [FromServices] IRemoveEducationCommand command,
            [FromQuery] Guid educationId)
        {
            return await command.Execute(educationId);
        }
    }
}
