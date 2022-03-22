using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Models.Position;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Broker.Requests.Interfaces
{
  [AutoInject]
  public interface IPositionService
  {
    Task CreateUserPositionAsync(Guid positionId, Guid userId, List<string> errors);

    Task<List<PositionData>> GetPositionsAsync(Guid userId, List<string> errors);
  }
}
