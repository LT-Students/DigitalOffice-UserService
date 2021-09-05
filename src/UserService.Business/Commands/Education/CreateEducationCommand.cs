using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Requests.Image;
using LT.DigitalOffice.Models.Broker.Responses.Image;
using LT.DigitalOffice.UserService.Business.Commands.Education.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Education;
using LT.DigitalOffice.UserService.Validation.Education.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace LT.DigitalOffice.UserService.Business.Commands.Education
{
  public class CreateEducationCommand : ICreateEducationCommand
  {
    private readonly IAccessValidator _accessValidator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDbUserEducationMapper _mapper;
    private readonly IUserRepository _userRepository;
    private readonly IEducationRepository _educationRepository;
    private readonly ICreateEducationRequestValidator _validator;
    private readonly IRequestClient<ICreateImagesRequest> _createImagesRequest;
    private readonly ILogger<CreateEducationCommand> _logger;

    public CreateEducationCommand(
    IAccessValidator accessValidator,
    IHttpContextAccessor httpContextAccessor,
    IDbUserEducationMapper mapper,
    IUserRepository userRepository,
    IEducationRepository educationRepository,
    ICreateEducationRequestValidator validator,
    IRequestClient<ICreateImagesRequest> createImagesRequest,
    ILogger<CreateEducationCommand> logger)
    {
      _accessValidator = accessValidator;
      _httpContextAccessor = httpContextAccessor;
      _mapper = mapper;
      _userRepository = userRepository;
      _educationRepository = educationRepository;
      _validator = validator;
      _createImagesRequest = createImagesRequest;
      _logger = logger;
    }

    public OperationResultResponse<Guid> Execute(CreateEducationRequest request)
    {
      Guid senderId = _httpContextAccessor.HttpContext.GetUserId();
      DbUser dbSender = _userRepository.Get(senderId);

      OperationResultResponse<Guid> response = new();

      if (!_accessValidator.HasRights(Rights.AddEditRemoveUsers))
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;

        response.Errors.Add("Not enough rights");
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

      DbUserEducation dbEducation = _mapper.Map(request);
      List<Guid> imagesIdsForCreate = CreateImages(request, response.Errors);

      if (imagesIdsForCreate is not null && imagesIdsForCreate.Any())
      {
        foreach(Guid createdImageId in imagesIdsForCreate)
        {
          dbEducation.Images.Add(new DbUserEducationImages()
          {
            ImageId = createdImageId,
            UserEducationId = dbEducation.Id
          });
        }
      }  

      _educationRepository.Add(dbEducation);

      _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
      response.Body = dbEducation.Id;
      response.Status = response.Errors.Any() ? 
        OperationResultStatusType.PartialSuccess : OperationResultStatusType.FullSuccess; 

      return response;
    }

    public List<Guid> CreateImages(CreateEducationRequest request, List<string> errors)
    {
      List<AddImageRequest> imagesToCreate = request.Images;
      Guid userId = request.UserId;

      if (imagesToCreate.Contains(null))
      {
        errors.Add($"Bad request to create education images for user {userId}");
        return null;
      }

      List<CreateImageData> images = new();

      foreach (AddImageRequest imageData in imagesToCreate)
      {
        images.Add(new CreateImageData(imageData.Name, imageData.Content, imageData.Extension, userId));
      }
      
      string logMsg = "Can not add education images to user {UserId}. Reason: '{Errors}'";

      try
      {
        Response<IOperationResult<ICreateImagesResponse>> response = _createImagesRequest.GetResponse<IOperationResult<ICreateImagesResponse>>(
          ICreateImagesRequest.CreateObj(images, ImageSource.User)
        ).Result;

        if(response.Message.IsSuccess)
        {
          return response.Message.Body.ImagesIds;
        }

        _logger.LogWarning(logMsg, userId, string.Join(',', response.Message.Errors));
      }
      catch (Exception exc)
      {
        _logger.LogError(logMsg, userId, exc.Message);
      }

      errors.Add($"Can not add education images to user with id {userId}");
      return null;
    }
  }
}
