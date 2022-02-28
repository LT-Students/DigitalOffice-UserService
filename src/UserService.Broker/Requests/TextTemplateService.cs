using LT.DigitalOffice.Kernel.BrokerSupport.Helpers;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Requests.TextTemplate;
using LT.DigitalOffice.Models.Broker.Responses.TextTemplate;
using LT.DigitalOffice.UserService.Broker.Requests.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Broker.Requests
{
  public class TextTemplateService : ITextTemplateService
  {
    private readonly IRequestClient<IGetTextTemplateRequest> _rcGetTextTemplate;
    private readonly ILogger<TextTemplateService> _logger;

    public TextTemplateService(
      IRequestClient<IGetTextTemplateRequest> rcGetTextTemplate,
      ILogger<TextTemplateService> logger)
    {
      _rcGetTextTemplate = rcGetTextTemplate;
      _logger = logger;
    }

    public async Task<IGetTextTemplateResponse> GetAsync(
      Guid endpointId,
      string locale,
      List<string> errors)
    {
      return await RequestHandler.ProcessRequest<IGetTextTemplateRequest, IGetTextTemplateResponse>(
        _rcGetTextTemplate,
        IGetTextTemplateRequest.CreateObj(
          endpointId: endpointId,
          templateType: TemplateType.Greeting,
          locale: locale),
        errors,
        _logger);
    }
  }
}
