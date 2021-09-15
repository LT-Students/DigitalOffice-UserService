﻿using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Requests.Image;
using LT.DigitalOffice.UserService.Business.Commands.Certificate.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Certificates;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Business.Commands.Certificate
{
  public class CreateCertificateCommand : ICreateCertificateCommand
  {
    private IAccessValidator _accessValidator;
    private IHttpContextAccessor _httpContextAccessor;
    private IUserRepository _userRepository;
    private ICertificateRepository _certificateRepository;
    private IDbUserCertificateMapper _mapper;
    private readonly ICreateImageDataMapper _createImageDataMapper;
    private readonly IRequestClient<ICreateImagesRequest> _rcImage;
    private readonly ILogger<CreateCertificateCommand> _logger;

    private Guid? GetImageId(AddImageRequest addImageRequest, List<string> errors)
    {
      Guid? imageId = null;

      if (addImageRequest == null)
      {
        return null;
      }

      const string errorMessage = "Can not add certificate image to certificate. Please try again later.";

      try
      {
        Response<IOperationResult<Guid>> response = _rcImage.GetResponse<IOperationResult<Guid>>(
          ICreateImagesRequest.CreateObj(
            _createImageDataMapper.Map(new List<AddImageRequest>() { addImageRequest },
            _httpContextAccessor.HttpContext.GetUserId()),
            ImageSource.User),
          default, TimeSpan.FromSeconds(5)).Result;
        if (!response.Message.IsSuccess)
        {
          _logger.LogWarning(
            errorMessage + "Reason:\n{Errors}",
            string.Join(',', response.Message.Errors));

          errors.Add(errorMessage);
        }
        else
        {
          imageId = response.Message.Body;
        }
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, errorMessage);

        errors.Add(errorMessage);
      }

      return imageId;
    }

    public CreateCertificateCommand(
      IAccessValidator accessValidator,
      IHttpContextAccessor httpContextAccessor,
      IDbUserCertificateMapper mapper,
      IUserRepository userRepository,
      ICertificateRepository certificateRepository,
      IRequestClient<ICreateImagesRequest> rcAddIImage,
      ICreateImageDataMapper createImageDataMapper,
      ILogger<CreateCertificateCommand> logger)
    {
      _accessValidator = accessValidator;
      _httpContextAccessor = httpContextAccessor;
      _mapper = mapper;
      _userRepository = userRepository;
      _certificateRepository = certificateRepository;
      _rcImage = rcAddIImage;
      _logger = logger;
      _createImageDataMapper = createImageDataMapper;
    }

    public OperationResultResponse<Guid> Execute(CreateCertificateRequest request)
    {
      List<string> errors = new();

      Guid senderId = _httpContextAccessor.HttpContext.GetUserId();

      if (!(_accessValidator.HasRights(Rights.AddEditRemoveUsers))
        && senderId != request.UserId)
      {
        throw new ForbiddenException("Not enough rights.");
      }

      Guid? imageId = GetImageId(request.Image, errors);

      if (!imageId.HasValue)
      {
        return new OperationResultResponse<Guid>
        {
          Status = OperationResultStatusType.Failed,
          Errors = errors
        };
      }

      DbUserCertificate dbUserCertificate = _mapper.Map(request, imageId.Value);

      _certificateRepository.Add(dbUserCertificate);

      return new OperationResultResponse<Guid>
      {
        Status = OperationResultStatusType.FullSuccess,
        Body = dbUserCertificate.Id
      };
    }
  }
}
