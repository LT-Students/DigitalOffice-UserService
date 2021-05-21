using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.UserService.Business.Commands.Certificate.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Certificates;
using LT.DigitalOffice.UserService.Models.Dto.Responses;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
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
        private readonly IUserRepository _repository;
        private readonly IPatchDbUserCertificateMapper _mapper;
        private readonly IRequestClient<IAddImageRequest> _rcImage;
        private readonly ILogger<EditCertificateCommand> _logger;

        private Guid? GetImageId(AddImageRequest addImageRequest, List<string> errors)
        {
            Guid? imageId = null;

            if (addImageRequest == null)
            {
                return imageId;
            }

            Guid userId = _httpContextAccessor.HttpContext.GetUserId();

            string errorMessage = "Can not add certificate image to certificate. Please try again later.";

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
                        errorMessage + $"Reason: '{string.Join(',', response.Message.Errors)}'");

                    throw new BadRequestException(errorMessage);
                }
                else
                {
                    imageId = response.Message.Body.Id;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, errorMessage);

                throw;
            }

            return imageId;
        }

        public EditCertificateCommand(
            IAccessValidator accessValidator,
            IHttpContextAccessor httpContextAccessor,
            IUserRepository repository,
            IPatchDbUserCertificateMapper mapper,
            IRequestClient<IAddImageRequest> rcImage,
            ILogger<EditCertificateCommand> logger)
        {
            _accessValidator = accessValidator;
            _httpContextAccessor = httpContextAccessor;
            _repository = repository;
            _mapper = mapper;
            _rcImage = rcImage;
            _logger = logger;
        }

        public OperationResultResponse<bool> Execute(Guid userId, Guid certificateId, JsonPatchDocument<EditCertificateRequest> request)
        {
            List<string> errors = new List<string>();

            var senderId = _httpContextAccessor.HttpContext.GetUserId();
            var sender = _repository.Get(senderId);
            if (!(sender.IsAdmin ||
                  _accessValidator.HasRights(Rights.AddEditRemoveUsers))
                  && senderId != userId)
            {
                throw new ForbiddenException("Not enough rights.");
            }

            DbUserCertificate certificate = _repository.GetCertificate(certificateId);
            if (certificate.UserId != userId)
            {
                throw new BadRequestException($"Certificate {certificateId} is not linked to this user {userId}");
            }

            var imageOperation = request.Operations.FirstOrDefault(o => o.path.EndsWith(nameof(EditCertificateRequest.Image), StringComparison.OrdinalIgnoreCase));
            Guid? imageId = null;
            if (imageOperation != null)
            {
                imageId = GetImageId(JsonConvert.DeserializeObject<AddImageRequest>(imageOperation.value?.ToString()), errors);
            }

            JsonPatchDocument<DbUserCertificate> dbRequest = _mapper.Map(request, imageId);

            bool result = _repository.EditCertificate(certificate, dbRequest);

            return new OperationResultResponse<bool>
            {
                Status = OperationResultStatusType.FullSuccess,
                Body = result
            };
        }
    }
}
