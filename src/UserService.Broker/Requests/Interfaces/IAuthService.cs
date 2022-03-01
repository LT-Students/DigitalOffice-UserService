using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Responses.Auth;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Broker.Requests.Interfaces
{
  [AutoInject]
  public interface IAuthService
  {
    Task<IGetTokenResponse> GetTokenAsync(Guid userId, List<string> errors);
  }
}
