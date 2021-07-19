﻿using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Business.Commands.Education.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Education;
using LT.DigitalOffice.UserService.Validation.Education.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Business.Commands.Education
{
    public class CreateEducationCommand : ICreateEducationCommand
    {
        private readonly IAccessValidator _accessValidator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDbUserEducationMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IEducationRepository _educationRepository;
        private readonly ICreateEducationRequestValidator _validator;

        public CreateEducationCommand(
            IAccessValidator accessValidator,
            IHttpContextAccessor httpContextAccessor,
            IDbUserEducationMapper mapper,
            IUserRepository userRepository,
            IEducationRepository educationRepository,
            ICreateEducationRequestValidator validator)
        {
            _accessValidator = accessValidator;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _userRepository = userRepository;
            _educationRepository = educationRepository;
            _validator = validator;
        }

        public OperationResultResponse<Guid> Execute(CreateEducationRequest request)
        {
            var senderId = _httpContextAccessor.HttpContext.GetUserId();
            var dbUser = _userRepository.Get(senderId);
            if (!(dbUser.IsAdmin ||
                  _accessValidator.HasRights(Rights.AddEditRemoveUsers))
                  && senderId != request.UserId)
            {
                throw new ForbiddenException("Not enough rights.");
            }

            _validator.ValidateAndThrowCustom(request);

            var dbEducation = _mapper.Map(request);

            _educationRepository.Add(dbEducation);

            return new OperationResultResponse<Guid>(
                dbEducation.Id,
                OperationResultStatusType.FullSuccess,
                new List<string>()
            );
        }
    }
}
