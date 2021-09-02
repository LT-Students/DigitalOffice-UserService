using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Business.Commands.Skill.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;

namespace LT.DigitalOffice.UserService.Business.Commands.Skill
{
    public class CreateSkillCommand : ICreateSkillCommand
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;
        private readonly IDbSkillMapper _mapper;
        private readonly ISkillRepository _skillRepository;
        private readonly IAccessValidator _accessValidator;

        public CreateSkillCommand(
            IHttpContextAccessor httpContextAccessor,
            IUserRepository userRepository,
            IDbSkillMapper mapper,
            ISkillRepository skillRepository,
            IAccessValidator accessValidator)
        {
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            _mapper = mapper;
            _skillRepository = skillRepository;
            _accessValidator = accessValidator;
        }

        public OperationResultResponse<Guid> Execute(CreateSkillRequest request)
        {
            OperationResultResponse<Guid> response = new();
            Guid senderId = _httpContextAccessor.HttpContext.GetUserId();
            DbUser dbUser = _userRepository.Get(senderId);

            if (!(dbUser.IsAdmin || _accessValidator.HasRights(Rights.AddEditRemoveUsers)))
            {
                response.Errors.Add("Not enough rights.");
                response.Status = OperationResultStatusType.Failed;
                _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;

                return response;
            }

            if (_skillRepository.DoesSkillAlreadyExist(request.Name))
            {
                response.Errors.Add($"Skill {request.Name} already exist");
                response.Status = OperationResultStatusType.Failed;
                _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;

                return response;
            }

            response.Body = _skillRepository.Add(_mapper.Map(request));

            if(response.Body == default)
            {
                _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;
                response.Status = OperationResultStatusType.Failed;
            }

            _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;
            response.Status = OperationResultStatusType.FullSuccess;

            return response;
        }
    }
}