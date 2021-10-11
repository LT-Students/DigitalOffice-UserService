using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Business.Commands.Communication.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Communication;
using LT.DigitalOffice.UserService.Validation.Communication.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.Communication
{
  public class CreateCommunicationCommand : ICreateCommunicationCommand
  {
    private readonly IUserRepository _userRepository;
    private readonly ICommunicationRepository _repository;
    private readonly IAccessValidator _accessValidator;
    private readonly IDbUserCommunicationMapper _mapper;
    private readonly ICreateCommunicationRequestValidator _validator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateCommunicationCommand(
      IUserRepository userRepository,
      ICommunicationRepository repository,
      IAccessValidator accessValidator,
      IDbUserCommunicationMapper mapper,
      ICreateCommunicationRequestValidator validator,
      IHttpContextAccessor httpContextAccessor)
    {
      _userRepository = userRepository;
      _repository = repository;
      _accessValidator = accessValidator;
      _mapper = mapper;
      _validator = validator;
      _httpContextAccessor = httpContextAccessor;
    }

    public async Task<OperationResultResponse<Guid>> ExecuteAsync(CreateCommunicationRequest request)
    {
      DbUser sender = _userRepository.Get(_httpContextAccessor.HttpContext.GetUserId());

      if (!(sender.IsAdmin
        || _accessValidator.HasRights(Rights.AddEditRemoveUsers)
        || request.UserId == _httpContextAccessor.HttpContext.GetUserId()))
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
        return new OperationResultResponse<Guid>
        {
          Status = OperationResultStatusType.Failed,
          Errors = new List<string>() { "Not enought rights" }
        };
      }

      _validator.ValidateAndThrowCustom(request);

      if (_repository.IsCommunicationValueExist(request.Value))
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;
        return new OperationResultResponse<Guid>
        {
          Status = OperationResultStatusType.Failed,
          Errors = new() { $"The communication '{request.Value}' already exists." }
        };
      }

      _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;
      return new OperationResultResponse<Guid>
      {
        Status = OperationResultStatusType.FullSuccess,
        Body = await _repository.AddAsync(_mapper.Map(request)),
        Errors = new()
      };
    }
  }
}
