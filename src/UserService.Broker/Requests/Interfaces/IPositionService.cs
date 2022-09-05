using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Models.Position;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Broker.Requests.Interfaces
{
  [AutoInject]
  public interface IPositionService
  {
    Task<List<PositionData>> GetPositionsAsync(Guid userId, List<string> errors, CancellationToken token);
  }
}
