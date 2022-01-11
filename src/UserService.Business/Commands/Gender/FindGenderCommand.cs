using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Kernel.Validators.Interfaces;
using LT.DigitalOffice.UserService.Business.Commands.Gender.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Gender.Filters;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.Gender
{
  public class FindGenderCommand : IFindGenderCommand
  {
    private readonly IGenderRepository _genderRepository;
    private readonly IGenderInfoMapper _mapper;
    private readonly IBaseFindFilterValidator _baseFindValidator;
    private readonly IResponseCreator _responseCreator;
    public FindGenderCommand(
      IBaseFindFilterValidator baseFindValidator,
      IGenderRepository genderRepository,
      IGenderInfoMapper mapper,
      IResponseCreator responseCreator)
    {
      _responseCreator = responseCreator;
      _baseFindValidator = baseFindValidator;
      _genderRepository = genderRepository;
      _mapper = mapper;
    }
    public async Task<FindResultResponse<GenderInfo>> ExecuteAsync(FindGendersFilter filter)
    {
      if (!_baseFindValidator.ValidateCustom(filter, out List<string> errors))
      {
        return _responseCreator.CreateFailureFindResponse<GenderInfo>(HttpStatusCode.BadRequest, errors);
      }

      FindResultResponse<GenderInfo> response = new();

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
