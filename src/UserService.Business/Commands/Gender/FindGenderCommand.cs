using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Business.Commands.Gender.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Gender.Filters;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.Gender
{
  public class FindGenderCommand : IFindGenderCommand
  {
    private readonly IGenderRepository _genderRepository;
    private readonly IGenderInfoMapper _mapper;

    public FindGenderCommand(
      IGenderRepository genderRepository,
      IGenderInfoMapper mapper)
    {
      _genderRepository = genderRepository;
      _mapper = mapper;
    }
    public async Task<FindResultResponse<GenderInfo>> ExecuteAsync(FindGendersFilter filter)
    {
      FindResultResponse<GenderInfo> response = new();
      response.Body = new();

      (List<DbGender> dbGenders, int totalCount) = await _genderRepository.FindGendersAsync(filter);

      response.TotalCount = totalCount;

      response.Body = _mapper.Map(dbGenders);

      response.Status = response.Errors.Any()
       ? OperationResultStatusType.PartialSuccess
       : OperationResultStatusType.FullSuccess;

      return response;
    }
  }
}
