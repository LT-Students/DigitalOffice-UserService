using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Business.Commands.Communication.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Communication;
using LT.DigitalOffice.UserService.Validation.Communication.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Business.Commands.Communication
{
    public class CreateCommunicationCommand : ICreateCommunicationCommand
    {
        private readonly IUserRepository _userRepository;
        private readonly ICommunicationRepository _repository;
        private readonly IAccessValidator _accessValidator;
        private readonly IDbUserCommunicationMapper _mapper;
        private readonly ICreateCommunicationRequestValidator _validator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CreateCommunicationCommand(
            IUserRepository userRepository,
            ICommunicationRepository repository,
            IAccessValidator accessValidator,
            IDbUserCommunicationMapper mapper,
            ICreateCommunicationRequestValidator validator,
            IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _repository = repository;
            _accessValidator = accessValidator;
            _mapper = mapper;
            _validator = validator;
            _httpContextAccessor = httpContextAccessor;
        }

        public OperationResultResponse<Guid> Execute(CreateCommunicationRequest request)
        {
            DbUser sender = _userRepository.Get(_httpContextAccessor.HttpContext.GetUserId());

            if (!(sender.IsAdmin
                || _accessValidator.HasRights(Rights.AddEditRemoveUsers)
                || request.UserId == _httpContextAccessor.HttpContext.GetUserId()))
            {
                throw new ForbiddenException("Not enough rights.");
            }

            _validator.ValidateAndThrowCustom(request);

            if (_repository.IsCommunicationValueExist(request.Value))
            {
                return new OperationResultResponse<Guid>(
                    Guid.Empty,
                    OperationResultStatusType.Conflict,
                    new() {$"The communication '{request.Value}' already exists."});
            }

            return new OperationResultResponse<Guid>(
                _repository.Add(_mapper.Map(request)),
                OperationResultStatusType.FullSuccess,
                new List<string>());
        }
    }
}
