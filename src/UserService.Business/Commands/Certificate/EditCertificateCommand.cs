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
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Certificates;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.UserService.Business.Commands.Certificate
{
  public class EditCertificateCommand : IEditCertificateCommand
  {
    private readonly IAccessValidator _accessValidator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRepository _userRepository;
    private readonly ICertificateRepository _certificateRepository;
    private readonly IPatchDbUserCertificateMapper _mapper;
    private readonly ICreateImageDataMapper _createImageDataMapper;
    private readonly IRequestClient<ICreateImagesRequest> _rcImage;
    private readonly ILogger<EditCertificateCommand> _logger;

    private Guid? GetImageId(AddImageRequest addImageRequest, List<string> errors)
    {
      Guid? imageId = null;

      if (addImageRequest == null)
      {
        return null;
      }

      Guid userId = _httpContextAccessor.HttpContext.GetUserId();

      const string errorMessage = "Can not add certificate image to certificate. Please try again later.";

      try
      {
        Response<IOperationResult<Guid>> response = _rcImage.GetResponse<IOperationResult<Guid>>(
          ICreateImagesRequest.CreateObj(_createImageDataMapper.Map(
            new List<AddImageRequest>() { addImageRequest }, userId),ImageSource.User), default, TimeSpan.FromSeconds(5)).Result;

        if (!response.Message.IsSuccess)
        {
          _logger.LogWarning("Can not add certificate image to certificate. Reason: '{Errors}'",
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

    public EditCertificateCommand(
      IAccessValidator accessValidator,
      IHttpContextAccessor httpContextAccessor,
      IUserRepository userRepository,
      ICertificateRepository certificateRepository,
      IPatchDbUserCertificateMapper mapper,
      ICreateImageDataMapper createImageDataMapper,
      IRequestClient<ICreateImagesRequest> rcImage,
      ILogger<EditCertificateCommand> logger)
    {
      _accessValidator = accessValidator;
      _httpContextAccessor = httpContextAccessor;
      _userRepository = userRepository;
      _certificateRepository = certificateRepository;
      _mapper = mapper;
      _createImageDataMapper = createImageDataMapper;
      _rcImage = rcImage;
      _logger = logger;
    }

    public OperationResultResponse<bool> Execute(Guid certificateId, JsonPatchDocument<EditCertificateRequest> request)
    {
      List<string> errors = new List<string>();

      Guid senderId = _httpContextAccessor.HttpContext.GetUserId();
      DbUser sender = _userRepository.Get(senderId);

      DbUserCertificate certificate = _certificateRepository.Get(certificateId);

      if (!(_accessValidator.HasRights(Rights.AddEditRemoveUsers))
        && senderId != certificate.UserId)
      {
        throw new ForbiddenException("Not enough rights.");
      }

      Operation<EditCertificateRequest> imageOperation = request.Operations
        .FirstOrDefault(o => o.path.EndsWith(nameof(EditCertificateRequest.Image), StringComparison.OrdinalIgnoreCase));

      Guid? imageId = null;

      if (imageOperation != null)
      {
        imageId = GetImageId(JsonConvert.DeserializeObject<AddImageRequest>(imageOperation.value?.ToString()), errors);
      }

      JsonPatchDocument<DbUserCertificate> dbRequest = _mapper.Map(request, imageId);

      bool result = _certificateRepository.Edit(certificate, dbRequest);

      return new OperationResultResponse<bool>
      {
        Status = errors.Any() ? OperationResultStatusType.PartialSuccess : OperationResultStatusType.FullSuccess,
        Body = result,
        Errors = errors
      };
    }
  }
}
