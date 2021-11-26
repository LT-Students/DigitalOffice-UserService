using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Business.Commands.Skill.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class SkillController : ControllerBase
  {
    [HttpPost("create")]
    public async Task<OperationResultResponse<Guid>> Create(
      [FromServices] ICreateSkillCommand command,
      [FromBody] CreateSkillRequest request)
    {
      return await command.ExecuteAsync(request);
    }
  }
}
