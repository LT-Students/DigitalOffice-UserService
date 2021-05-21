using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.UserService.Business.Commands.Certificate.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Responses;
using Microsoft.AspNetCore.Http;
using System;

namespace LT.DigitalOffice.UserService.Business.Commands.Certificate
{
    public class RemoveCertificateCommand : IRemoveCertificateCommand
    {
        private readonly IAccessValidator _accessValidator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _repository;

        public RemoveCertificateCommand(
            IAccessValidator accessValidator,
            IHttpContextAccessor httpContextAccessor,
            IUserRepository repository)
        {
            _accessValidator = accessValidator;
            _httpContextAccessor = httpContextAccessor;
            _repository = repository;
        }

        public OperationResultResponse<bool> Execute(Guid userId, Guid certificateId)
        {
            var senderId = _httpContextAccessor.HttpContext.GetUserId();
            var sender = _repository.Get(senderId);
            if (!(sender.IsAdmin ||
                  _accessValidator.HasRights(Rights.AddEditRemoveUsers))
                  && senderId != userId)
            {
                throw new ForbiddenException("Not enough rights.");
            }

            DbUserCertificate userCertificate = _repository.GetCertificate(certificateId);

            if (userCertificate.UserId != userId)
            {
                throw new BadRequestException($"Certificate {certificateId} is not linked to this user {userId}");
            }

            bool result = _repository.RemoveCertificate(userCertificate);

            return new OperationResultResponse<bool>
            {
                Status = OperationResultStatusType.FullSuccess,
                Body = result
            };
        }
    }
}
