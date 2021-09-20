using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Achievement;
using Microsoft.AspNetCore.Mvc;
using System;


namespace LT.DigitalOffice.UserService.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class AchievementController
  {
    [HttpPost("create")]
    public OperationResultResponse<Guid> Create(
    [FromServices] ICreateAchievementCommand command,
    [FromBody] CreateAchievementRequest request)
    {
      return command.Execute(request);
    }
  }
}
