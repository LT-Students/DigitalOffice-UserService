using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Business.Commands.Communication.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Communication;
using LT.DigitalOffice.UserService.Validation.Communication.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using System;

namespace LT.DigitalOffice.UserService.Business.Commands.Communication
{
    public class EditCommunicationCommand : IEditCommunicationCommand
    {
        private readonly IUserRepository _userRepository;
        private readonly ICommunicationRepository _repository;
        private readonly IAccessValidator _accessValidator;
        private readonly IPatchDbUserCommunicationMapper _mapper;
        private readonly IEditCommunicationRequestValidator _validator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EditCommunicationCommand(
            IUserRepository userRepository,
            ICommunicationRepository repository,
            IAccessValidator accessValidator,
            IPatchDbUserCommunicationMapper mapper,
            IEditCommunicationRequestValidator validator,
            IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _repository = repository;
            _accessValidator = accessValidator;
            _mapper = mapper;
            _validator = validator;
            _httpContextAccessor = httpContextAccessor;
        }
        public OperationResultResponse<bool> Execute(
            Guid communicationId, 
            JsonPatchDocument<EditCommunicationRequest> request)
        {
            var senderId = _httpContextAccessor.HttpContext.GetUserId();
            var sender = _userRepository.Get(senderId);

            DbUserCommunication communication = _repository.Get(communicationId);

            if (!(sender.IsAdmin ||
                  _accessValidator.HasRights(Rights.AddEditRemoveUsers))
                  && senderId != communication.UserId)
            {
                throw new ForbiddenException("Not enough rights.");
            }

            _validator.ValidateAndThrowCustom(request);

            return new OperationResultResponse<bool>
            {
                Status = OperationResultStatusType.FullSuccess,
                Body = _repository.Edit(communicationId, _mapper.Map(request)),
                Errors = new()
            };
        }
    }
}
