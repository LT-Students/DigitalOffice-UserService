﻿using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Business.Commands.Achievement.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Achievement;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Achievement.Filters;
using LT.DigitalOffice.UserService.Models.Dto.Responses.Achievement;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;

namespace LT.DigitalOffice.UserService.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class AchievementController
  {
    [HttpPost("create")]
    public OperationResultResponse<Guid?> Create(
      [FromServices] ICreateAchievementCommand command,
      [FromBody] CreateAchievementRequest request)
    {
      return command.Execute(request);
    }

    [HttpPatch("edit")]
    public OperationResultResponse<bool> Edit(
      [FromServices] IEditAchievementCommand command,
      [FromQuery] Guid achievementId,
      [FromBody] JsonPatchDocument<EditAchievementRequest> request)
    {
      return command.Execute(achievementId, request);
    }

    [HttpGet("find")]
    public FindResultResponse<AchievementInfo> Find(
      [FromServices] IFindAchievementCommand command,
      [FromQuery] FindAchievementFilter filter)
    {
      return command.Execute(filter);
    }

    [HttpGet("get")]
    public OperationResultResponse<AchievementResponse> Get(
      [FromServices] IGetAchievementCommand command,
      [FromQuery] Guid achievementId)
    {
      return command.Execute(achievementId);
    }
  }
}
