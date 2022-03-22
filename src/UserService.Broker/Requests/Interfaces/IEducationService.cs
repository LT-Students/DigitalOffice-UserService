using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Models.Education;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Broker.Requests.Interfaces
{
  [AutoInject]
  public interface IEducationService
  {
    Task<List<EducationData>> GetEducationsAsync(
      Guid userId,
      List<string> errors);
  }
}
