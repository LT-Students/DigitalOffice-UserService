using FluentValidation;
using FluentValidation.Results;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Business.Commands.Password.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Helpers.Password;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Credentials.Filters;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Password;
using LT.DigitalOffice.UserService.Validation.Password.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.Password
{
  public class ChangePasswordCommand : IChangePasswordCommand
  {
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IPasswordValidator _validator;
    private readonly IUserCredentialsRepository _repository;
    private readonly IResponseCreator _responseCreator;

    public ChangePasswordCommand(
      IHttpContextAccessor httpContextAccessor,
      IPasswordValidator validator,
      IUserCredentialsRepository repository,
      IResponseCreator responseCreator)
    {
      _httpContextAccessor = httpContextAccessor;
      _validator = validator;
      _repository = repository;
      _responseCreator = responseCreator;
    }

    public async Task<OperationResultResponse<bool>> ExecuteAsync(ChangePasswordRequest request)
    {
      ValidationResult validationResult = await _validator.ValidateAsync(request.NewPassword);

      if (!validationResult.IsValid)
      {
        return _responseCreator.CreateFailureResponse<bool>(
          HttpStatusCode.BadRequest,
          validationResult.Errors.Select(vf => vf.ErrorMessage).ToList());
      }

      DbUserCredentials dbUserCredentials = await _repository
        .GetAsync(new GetCredentialsFilter() { UserId = _httpContextAccessor.HttpContext.GetUserId() });

      if (dbUserCredentials is null)
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.NotFound);
      }

      if (dbUserCredentials.PasswordHash
        != UserPasswordHash.GetPasswordHash(dbUserCredentials.Login, dbUserCredentials.Salt, request.Password))
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.BadRequest);
      }

      dbUserCredentials.PasswordHash = UserPasswordHash.GetPasswordHash(
        dbUserCredentials.Login,
        dbUserCredentials.Salt,
        request.NewPassword);

      return new OperationResultResponse<bool>(
        body: await _repository.EditAsync(dbUserCredentials));
    }
  }
}
