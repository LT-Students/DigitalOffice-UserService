using System;
using FluentValidation;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.UserService.Business
{
    public class UserCreateCommand : IUserCreateCommand
    {
        private readonly IValidator<UserCreateRequest> validator;
        private readonly IUserRepository repository;
        private readonly IMapper<UserCreateRequest, DbUser> mapper;

        public UserCreateCommand(
            [FromServices] IValidator<UserCreateRequest> validator,
            [FromServices] IUserRepository repository,
            [FromServices] IMapper<UserCreateRequest, DbUser> mapper)
        {
            this.validator = validator;
            this.repository = repository;
            this.mapper = mapper;
        }

        public Guid Execute(UserCreateRequest request)
        {
            validator.ValidateAndThrow(request);
            var user = mapper.Map(request);

            return repository.UserCreate(user);
        }
    }
}