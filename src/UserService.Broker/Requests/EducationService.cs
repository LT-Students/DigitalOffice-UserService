using LT.DigitalOffice.Kernel.BrokerSupport.Helpers;
using LT.DigitalOffice.Models.Broker.Models.Education;
using LT.DigitalOffice.Models.Broker.Requests.Education;
using LT.DigitalOffice.Models.Broker.Responses.Education;
using LT.DigitalOffice.UserService.Broker.Requests.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Broker.Requests
{
  public class EducationService : IEducationService
  {
    private readonly IRequestClient<IGetUserEducationsRequest> _rcGetUserEducations;
    private readonly ILogger<EducationService> _logger;

    public EducationService(
      IRequestClient<IGetUserEducationsRequest> rcGetUserEducations,
      ILogger<EducationService> logger)
    {
      _rcGetUserEducations = rcGetUserEducations;
      _logger = logger;
    }

    public async Task<List<EducationData>> GetEducationsAsync(
      Guid userId,
      List<string> errors)
    {
      return (await RequestHandler.ProcessRequest<IGetUserEducationsRequest, IGetUserEducationsResponse>(
        _rcGetUserEducations,
        IGetUserEducationsRequest.CreateObj(userId: userId),
        errors,
        _logger))?.Educations;
    }
  }
}
