using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Business.Commands.Skill.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using LT.DigitalOffice.UserService.Validation.Skill.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.Skill
{
  public class CreateSkillCommand : ICreateSkillCommand
  {
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRepository _userRepository;
    private readonly IDbSkillMapper _mapper;
    private readonly ISkillRepository _skillRepository;
    private readonly IAccessValidator _accessValidator;
    private readonly ICreateSkillRequestValidator _validator;

    public CreateSkillCommand(
      IHttpContextAccessor httpContextAccessor,
      IUserRepository userRepository,
      IDbSkillMapper mapper,
      ISkillRepository skillRepository,
      IAccessValidator accessValidator,
      ICreateSkillRequestValidator validator)
    {
      _httpContextAccessor = httpContextAccessor;
      _userRepository = userRepository;
      _mapper = mapper;
      _skillRepository = skillRepository;
      _accessValidator = accessValidator;
      _validator = validator;
    }

    public async Task<OperationResultResponse<Guid>> Execute(CreateSkillRequest request)
    {
      OperationResultResponse<Guid> response = new();
      Guid senderId = _httpContextAccessor.HttpContext.GetUserId();
      DbUser dbUser = _userRepository.Get(senderId);

      if (!(dbUser.IsAdmin || _accessValidator.HasRights(Rights.AddEditRemoveUsers)))
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;

        response.Errors.Add("Not enough rights.");
        response.Status = OperationResultStatusType.Failed;

        return response;
      }

      if (!_validator.ValidateCustom(request, out List<string> errors))
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        response.Errors.AddRange(errors);
        response.Status = OperationResultStatusType.Failed;

        return response;
      }

      if (_skillRepository.DoesSkillAlreadyExist(request.Name))
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;

        response.Errors.Add($"Skill {request.Name} already exist");
        response.Status = OperationResultStatusType.Failed;

        return response;
      }

      response.Body = await _skillRepository.Add(_mapper.Map(request));
      response.Status = OperationResultStatusType.FullSuccess;

      _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;

      if (response.Body == default)
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        response.Status = OperationResultStatusType.Failed;
      }

      return response;
    }
  }
}