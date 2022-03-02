using LT.DigitalOffice.Kernel.BrokerSupport.Helpers;
using LT.DigitalOffice.Models.Broker.Requests.Auth;
using LT.DigitalOffice.Models.Broker.Responses.Auth;
using LT.DigitalOffice.UserService.Broker.Requests.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Broker.Requests
{
  public class AuthService : IAuthService
  {
    private readonly IRequestClient<IGetTokenRequest> _rcGetToken;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
      IRequestClient<IGetTokenRequest> rcGetToken,
      ILogger<AuthService> logger)
    {
      _rcGetToken = rcGetToken;
      _logger = logger;
    }

    public async Task<IGetTokenResponse> GetTokenAsync(Guid userId, List<string> errors)
    {
      return await RequestHandler.ProcessRequest<IGetTokenRequest, IGetTokenResponse>(
        _rcGetToken,
        IGetTokenRequest.CreateObj(userId),
        errors,
        _logger);
    }
  }
}
