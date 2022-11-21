using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Models.Office;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Broker.Requests.Interfaces
{
  [AutoInject]
  public interface IOfficeService
  {
    Task<List<OfficeData>> GetOfficesAsync(Guid userId, List<string> errors, CancellationToken cancellationToken = default);
  }
}
