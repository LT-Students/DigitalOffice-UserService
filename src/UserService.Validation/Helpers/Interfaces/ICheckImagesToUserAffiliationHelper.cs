using LT.DigitalOffice.Kernel.Attributes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Validation.Helpers.Interfaces
{
  [AutoInject]
  public interface ICheckImagesToUserAffiliationHelper
  {
    Task<bool> CheckAffiliationAsync(List<Guid> imagesIds, Guid userId);
  }
}
