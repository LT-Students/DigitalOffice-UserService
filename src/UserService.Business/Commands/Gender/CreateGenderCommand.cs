﻿using FluentValidation.Results;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Business.Commands.User.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Gender;
using LT.DigitalOffice.UserService.Validation.Gender.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.User
{
  public class CreateGenderCommand : ICreateGenderCommand
  {
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDbGenderMapper _mapper;
    private readonly IGenderRepository _genderRepository;
    private readonly ICreateGenderRequestValidator _validator;
    private readonly IResponseCreator _responseCreator;
    private readonly IAccessValidator _accessValidator;

    public CreateGenderCommand(
      IHttpContextAccessor httpContextAccessor,
      IDbGenderMapper mapper,
      IGenderRepository genderRepository,
      ICreateGenderRequestValidator validator,
      IResponseCreator responseCreator,
      IAccessValidator accessValidator)
    {
      _httpContextAccessor = httpContextAccessor;
      _mapper = mapper;
      _genderRepository = genderRepository;
      _validator = validator;
      _responseCreator = responseCreator;
      _accessValidator = accessValidator;
    }

    public async Task<OperationResultResponse<Guid?>> ExecuteAsync(CreateGenderRequest request)
    {
      if (!await _accessValidator.IsAdminAsync(_httpContextAccessor.HttpContext.GetUserId()))
      {
        return _responseCreator.CreateFailureResponse<Guid?>(HttpStatusCode.Forbidden);
      }

      ValidationResult validationResult = await _validator.ValidateAsync(request);

      if (!validationResult.IsValid)
      {
        return _responseCreator.CreateFailureResponse<Guid?>(HttpStatusCode.BadRequest,
          validationResult.Errors.Select(vf => vf.ErrorMessage).ToList());
      }

      DbGender dbGender = _mapper.Map(request);
      await _genderRepository.CreateAsync(dbGender);

      _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;

      return new OperationResultResponse<Guid?>(
        body: dbGender.Id);
    }
  }
}
