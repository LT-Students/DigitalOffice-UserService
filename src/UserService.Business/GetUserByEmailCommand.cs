﻿using FluentValidation;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Interfaces;
using Microsoft.AspNetCore.Mvc;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;

namespace LT.DigitalOffice.UserService.Business
{
    public class GetUserByEmailCommand : IGetUserByEmailCommand
    {
        private readonly IValidator<string> validator;
        private readonly IUserRepository repository;
        private readonly IMapper<DbUser, User> mapper;

        public GetUserByEmailCommand(
            [FromServices] IValidator<string> validator,
            [FromServices] IUserRepository repository,
            [FromServices] IMapper<DbUser, User> mapper)
        {
            this.validator = validator;
            this.repository = repository;
            this.mapper = mapper;
        }

        public User Execute(string userEmail)
        {
            validator.ValidateAndThrow(userEmail);

            return mapper.Map(repository.GetUserByEmail(userEmail));
        }
    }
}
