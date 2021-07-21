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
using Microsoft.AspNetCore.JsonPatch.Operations;
using System;
using System.Linq;

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
            Guid senderId = _httpContextAccessor.HttpContext.GetUserId();
            DbUser sender = _userRepository.Get(senderId);

            DbUserCommunication communication = _repository.Get(communicationId);

            if (!(sender.IsAdmin ||
                  _accessValidator.HasRights(Rights.AddEditRemoveUsers))
                  && senderId != communication.UserId)
            {
                throw new ForbiddenException("Not enough rights.");
            }

            _validator.ValidateAndThrowCustom(request);

            Operation<EditCommunicationRequest> valueOperation = request.Operations.FirstOrDefault(
                o => o.path.EndsWith(nameof(EditCommunicationRequest.Value), StringComparison.OrdinalIgnoreCase));

            if (valueOperation != null && _repository.IsCommunicationValueExist(valueOperation.value.ToString()))
            {
                return new OperationResultResponse<bool>
                {
                    Status = OperationResultStatusType.Conflict,
                    Errors = new() { $"The communication '{valueOperation.value}' already exists." }
                };
            }

            return new OperationResultResponse<bool>
            {
                Status = OperationResultStatusType.FullSuccess,
                Body = _repository.Edit(communicationId, _mapper.Map(request)),
                Errors = new()
            };
        }
    }
}
