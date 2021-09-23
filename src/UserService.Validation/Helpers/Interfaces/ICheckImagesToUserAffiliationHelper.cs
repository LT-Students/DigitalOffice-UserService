using LT.DigitalOffice.Kernel.Attributes;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Validation.Helpers.Interfaces
{
  [AutoInject]
  public interface ICheckImagesToUserAffiliationHelper
  {
    bool CheckAffiliation(List<Guid> imagesIds, Guid userId);
  }
}
