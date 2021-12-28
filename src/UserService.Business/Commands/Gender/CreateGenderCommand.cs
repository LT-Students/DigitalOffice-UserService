using FluentValidation.Results;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Business.Commands.User.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
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

    public CreateGenderCommand(
      IHttpContextAccessor httpContextAccessor,
      IDbGenderMapper mapper,
      IGenderRepository genderRepository,
      ICreateGenderRequestValidator validator,
      IResponseCreator responseCreator)
    {
      _httpContextAccessor = httpContextAccessor;
      _mapper = mapper;
      _genderRepository = genderRepository;
      _validator = validator;
      _responseCreator = responseCreator;
    }

    public async Task<OperationResultResponse<Guid?>> ExecuteAsync(CreateGenderRequest request)
    {
      OperationResultResponse<Guid?> response = new();

      ValidationResult validationResult = await _validator.ValidateAsync(request);

      if (!validationResult.IsValid)
      {
        return _responseCreator.CreateFailureResponse<Guid?>(HttpStatusCode.BadRequest,
          validationResult.Errors.Select(vf => vf.ErrorMessage).ToList());
      }

      response.Body = await _genderRepository.CreateAsync(_mapper.Map(request));
      response.Status = OperationResultStatusType.FullSuccess;

      _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;

      if (response.Body == default)
      {
        response = _responseCreator.CreateFailureResponse<Guid?>(HttpStatusCode.BadRequest);
      }

      return response;
    }
  }
}
