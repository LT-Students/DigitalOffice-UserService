using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Kernel.Validators.Interfaces;
using LT.DigitalOffice.UserService.Business.Commands.Achievement.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Achievement.Filters;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace LT.DigitalOffice.UserService.Business.Commands.Achievement
{
  public class FindAchievementCommand : IFindAchievementCommand
  {
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAchievementRepository _repository;
    private readonly IAchievementInfoMapper _mapper;
    private readonly IBaseFindFilterValidator _baseFindValidator;

    public FindAchievementCommand(
      IHttpContextAccessor httpContextAccessor,
      IAchievementRepository repository,
      IAchievementInfoMapper mapper,
      IBaseFindFilterValidator baseFindValidator)
    {
      _httpContextAccessor = httpContextAccessor;
      _repository = repository;
      _mapper = mapper;
      _baseFindValidator = baseFindValidator;
    }

    public FindResultResponse<AchievementInfo> Execute(FindAchievementFilter filter)
    {
      FindResultResponse<AchievementInfo> response = new();

      if (!_baseFindValidator.ValidateCustom(filter, out List<string> errors))
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        response.Status = OperationResultStatusType.Failed;
        response.Errors = errors;
        return response;
      }

      response.Body = _repository
        .Find(filter, out int totalCount)
        .Select(_mapper.Map)
        .ToList();

      response.TotalCount = totalCount;
      response.Status = OperationResultStatusType.FullSuccess;

      return response;
    }
  }
}
