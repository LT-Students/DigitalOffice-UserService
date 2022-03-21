using LT.DigitalOffice.Kernel.BrokerSupport.Helpers;
using LT.DigitalOffice.Models.Broker.Models.Skill;
using LT.DigitalOffice.Models.Broker.Requests.Skill;
using LT.DigitalOffice.Models.Broker.Responses.Skill;
using LT.DigitalOffice.UserService.Broker.Requests.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Broker.Requests
{
  public class SkillService : ISkillService
  {
    private readonly IRequestClient<IGetUserSkillsRequest> _rcGetSkills;
    private readonly ILogger<SkillService> _logger;

    public SkillService(
      IRequestClient<IGetUserSkillsRequest> rcGetSkills,
      ILogger<SkillService> logger)
    {
      _rcGetSkills = rcGetSkills;
      _logger = logger;
    }

    public async Task<List<UserSkillData>> GetSkillsAsync(Guid userId, List<string> errors)
    {
      return (await RequestHandler.ProcessRequest<IGetUserSkillsRequest, IGetUserSkillsResponse>(
          _rcGetSkills,
          IGetUserSkillsRequest.CreateObj(userId: userId),
          errors,
          _logger))
        ?.Skills;
    }
  }
}
