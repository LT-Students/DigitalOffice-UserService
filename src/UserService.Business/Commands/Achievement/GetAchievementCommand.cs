using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Business.Commands.Achievement.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Responses.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Responses.Achievement;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;

namespace LT.DigitalOffice.UserService.Business.Commands.Achievement
{
  public class GetAchievementCommand : IGetAchievementCommand
  {
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAchievementRepository _repository;
    private readonly IAchievementResponseMapper _mapper;

    public GetAchievementCommand(
      IHttpContextAccessor httpContextAccessor,
      IAchievementRepository repository,
      IAchievementResponseMapper mapper)
    {
      _httpContextAccessor = httpContextAccessor;
      _repository = repository;
      _mapper = mapper;
    }

    public OperationResultResponse<AchievementResponse> Execute(Guid achievementId)
    {
      OperationResultResponse<AchievementResponse> response = new();

      response.Body = _mapper.Map(_repository.Get(achievementId));
      response.Status = OperationResultStatusType.FullSuccess;

      if (response.Body == null)
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;

        response.Errors = new() { "Achievement was not found." };
        response.Status = OperationResultStatusType.Failed;
      }

      return response;
    }
  }
}
