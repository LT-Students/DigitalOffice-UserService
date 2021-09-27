﻿using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Requests.Image;
using LT.DigitalOffice.Models.Broker.Responses.Image;
using LT.DigitalOffice.UserService.Business.Commands.Certificate.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Certificates;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.Certificate
{
  public class CreateCertificateCommand : ICreateCertificateCommand
  {
    private IAccessValidator _accessValidator;
    private IHttpContextAccessor _httpContextAccessor;
    private IUserRepository _userRepository;
    private ICertificateRepository _certificateRepository;
    private IDbUserCertificateMapper _mapper;
    private readonly IRequestClient<ICreateImagesRequest> _rcImage;
    private readonly ILogger<CreateCertificateCommand> _logger;

    private async Task<List<Guid>> CreateImages(CreateCertificateRequest request, Guid userId, List<string> errors)
    {
      List<AddImageRequest> requestedImages = request.Images;
      if (requestedImages == null || !requestedImages.Any())
      {
        return new();
      }

      string logMessage = "Can not add certificate image for user {UserId}. Reason: {Errors}";

      List<CreateImageData> images = requestedImages
        .Select(i => new CreateImageData(i.Name, i.Content, i.Extension, userId))
        .ToList();

      try
      {
        IOperationResult<ICreateImagesResponse> responsedMsg = 
          (await _rcImage.GetResponse<IOperationResult<ICreateImagesResponse>>(
            ICreateImagesRequest.CreateObj(images, ImageSource.User))).Message;

        if (responsedMsg.IsSuccess)
        {
          return responsedMsg.Body.ImagesIds;
        }

        _logger.LogWarning(logMessage, userId, string.Join(',', responsedMsg.Errors));
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, logMessage, userId, exc.Message);
      }

      errors.Add($"Can not add certificate image for user {userId}. Please try again later.");

      return null;
    }

    public CreateCertificateCommand(
      IAccessValidator accessValidator,
      IHttpContextAccessor httpContextAccessor,
      IDbUserCertificateMapper mapper,
      IUserRepository userRepository,
      ICertificateRepository certificateRepository,
      IRequestClient<ICreateImagesRequest> rcAddIImage,
      ILogger<CreateCertificateCommand> logger)
    {
      _accessValidator = accessValidator;
      _httpContextAccessor = httpContextAccessor;
      _mapper = mapper;
      _userRepository = userRepository;
      _certificateRepository = certificateRepository;
      _rcImage = rcAddIImage;
      _logger = logger;
    }

    public async Task<OperationResultResponse<Guid>> Execute(CreateCertificateRequest request)
    {
      OperationResultResponse<Guid> response = new();
      Guid senderId = _httpContextAccessor.HttpContext.GetUserId();

      if (!_accessValidator.HasRights(Rights.AddEditRemoveUsers) && senderId != request.UserId)
      {
        response.Errors.Add("Not enough rights");

        response.Status = OperationResultStatusType.Failed;
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;

        return response;
      }

      List<Guid> createdImagesIds = await CreateImages(request, senderId, response.Errors);
      DbUserCertificate dbUserCertificate = _mapper.Map(request, createdImagesIds);

      _certificateRepository.Add(dbUserCertificate);

      response.Body = dbUserCertificate.Id;
      response.Status = response.Errors.Any() ?
        OperationResultStatusType.PartialSuccess : OperationResultStatusType.FullSuccess;
      _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;

      return response;
    }
  }
}
