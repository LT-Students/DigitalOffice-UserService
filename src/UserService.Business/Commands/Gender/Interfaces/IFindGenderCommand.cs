using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Gender.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.Gender.Interfaces
{
  [AutoInject]
  public interface IFindGenderCommand
  {
    Task<FindResultResponse<GenderInfo>> ExecuteAsync(FindGendersFilter filter);
  }
}
