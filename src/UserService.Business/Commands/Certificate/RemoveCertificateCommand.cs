using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Business.Commands.Certificate.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Business.Commands.Certificate
{
    public class RemoveCertificateCommand : IRemoveCertificateCommand
    {
        private readonly IAccessValidator _accessValidator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;
        private readonly ICertificateRepository _certificateRepository;

        public RemoveCertificateCommand(
            IAccessValidator accessValidator,
            IHttpContextAccessor httpContextAccessor,
            IUserRepository userRepository,
            ICertificateRepository certificateRepostory)
        {
            _accessValidator = accessValidator;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            _certificateRepository = certificateRepostory;
        }

        public OperationResultResponse<bool> Execute(Guid certificateId)
        {
            var senderId = _httpContextAccessor.HttpContext.GetUserId();
            var sender = _userRepository.Get(senderId);
            DbUserCertificate userCertificate = _certificateRepository.Get(certificateId);

            if (!(sender.IsAdmin ||
                  _accessValidator.HasRights(Rights.AddEditRemoveUsers))
                  && senderId != userCertificate.UserId)
            {
                throw new ForbiddenException("Not enough rights.");
            }

            bool result = _certificateRepository.Remove(userCertificate);

            return new OperationResultResponse<bool>(
                result,
                OperationResultStatusType.FullSuccess,
                new List<string>());
        }
    }
}
