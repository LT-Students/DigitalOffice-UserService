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

namespace LT.DigitalOffice.UserService.Business.Commands.Skill
{
    public class CreateSkillCommand : ICreateSkillCommand
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;
        private readonly IDbSkillMapper _mapper;
        private readonly ISkillRepository _skillRepository;
        private readonly IAccessValidator _accessValidator;

        public CreateSkillCommand
            (
                IHttpContextAccessor httpContextAccessor,
                IUserRepository userRepository,
                IDbSkillMapper mapper,
                ISkillRepository skillRepository,
                IAccessValidator accessValidator
            )
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

                return response;
            }

            if (_skillRepository.DoesSkillAlreadyExist(request.Name))
            {
                response.Errors.Add($"Skill {request.Name} already exist");
                response.Status = OperationResultStatusType.Conflict;

                return response;
            }

            DbSkill skill = _mapper.Map(request);

            _skillRepository.Add(skill);

            response.Status = OperationResultStatusType.FullSuccess;
            response.Body = skill.Id;

            return response;
        }
    }
}