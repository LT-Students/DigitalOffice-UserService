using System;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Common;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Http;

namespace LT.DigitalOffice.UserService.Business.Commands.User
{
    /// <inheritdoc/>
    public class DisableUserCommand : IDisableUserCommand
    {
        private readonly IUserRepository _repository;
        private readonly IAccessValidator _accessValidator;
        private readonly IBus _bus;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DisableUserCommand(
            IUserRepository repository,
            IAccessValidator accessValidator,
            IBus bus,
            IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _accessValidator = accessValidator;
            _bus = bus;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <inheritdoc/>
        public OperationResultResponse<bool> Execute(Guid userId)
        {
            if (!(_accessValidator.IsAdmin() || _accessValidator.HasRights(Rights.AddEditRemoveUsers)))
            {
                throw new ForbiddenException("Not enough rights.");
            }

            _bus.Publish<IDisactivateUserRequest>(IDisactivateUserRequest.CreateObj(
                userId,
                _httpContextAccessor.HttpContext.GetUserId()));

            return new OperationResultResponse<bool>
            {
                Status = OperationResultStatusType.FullSuccess,
                Body = _repository.SwitchActiveStatus(userId, false)
            };
        }
    }
}
