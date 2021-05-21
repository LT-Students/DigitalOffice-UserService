﻿using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.UserService.Business.Commands.Certificate.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Certificates;
using LT.DigitalOffice.UserService.Models.Dto.Responses;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;

namespace LT.DigitalOffice.UserService.Business.Commands.Certificate
{
    public class CreateCertificateCommand : ICreateCertificateCommand
    {
        private IAccessValidator _accessValidator;
        private IHttpContextAccessor _httpContextAccessor;
        private IUserRepository _repository;
        private IDbUserCertificateMapper _mapper;
        private readonly IRequestClient<IAddImageRequest> _rcImage;
        private readonly ILogger<CreateCertificateCommand> _logger;


        private Guid GetImageId(AddImageRequest addImageRequest)
        {
            Guid imageId;

            if (addImageRequest == null)
            {
                throw new ArgumentNullException(nameof(addImageRequest));
            }

            Guid userId = _httpContextAccessor.HttpContext.GetUserId();

            string errorMessage = "Can not add certificate image to certificate of user with id {userId}. Please try again later.";

            try
            {
                var response = _rcImage.GetResponse<IOperationResult<IAddImageResponse>>(
                    IAddImageRequest.CreateObj(
                        addImageRequest.Name,
                        addImageRequest.Content,
                        addImageRequest.Extension,
                        userId)).Result;

                if (!response.Message.IsSuccess)
                {
                    _logger.LogWarning(
                        errorMessage + $"Reason: '{string.Join(',', response.Message.Errors)}'", userId);

                    throw new BadRequestException(response.Message.Errors);
                }
                else
                {
                    imageId = response.Message.Body.Id;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, errorMessage, userId);

                throw;
            }

            return imageId;
        }

        public CreateCertificateCommand(
            IAccessValidator accessValidator,
            IHttpContextAccessor httpContextAccessor,
            IDbUserCertificateMapper mapper,
            IUserRepository userRepository,
            IRequestClient<IAddImageRequest> rcAddIImage,
            ILogger<CreateCertificateCommand> logger)
        {
            _accessValidator = accessValidator;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _repository = userRepository;
            _rcImage = rcAddIImage;
            _logger = logger;
        }

        public OperationResultResponse<Guid> Execute(CreateCertificateRequest request)
        {
            var senderId = _httpContextAccessor.HttpContext.GetUserId();
            var dbUser = _repository.Get(senderId);
            if (!(dbUser.IsAdmin ||
                  _accessValidator.HasRights(Rights.AddEditRemoveUsers))
                  && senderId != request.UserId)
            {
                throw new ForbiddenException("Not enough rights.");
            }

            var dbUserCertificate = _mapper.Map(request, GetImageId(request.Image));

            _repository.AddCertificate(dbUserCertificate);

            return new OperationResultResponse<Guid>
            {
                Status = OperationResultStatusType.FullSuccess,
                Body = dbUserCertificate.Id
            };
        }
    }
}
