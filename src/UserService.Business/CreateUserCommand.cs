using FluentValidation;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using LT.DigitalOffice.UserService.Models.Db;

namespace LT.DigitalOffice.UserService.Business
{
    public class CreateUserCommand : ICreateUserCommand
    {
        private readonly IValidator<CreateUserRequest> validator;
        private readonly IUserRepository repository;
        private readonly IMapper<CreateUserRequest, DbUser> mapper;

        public CreateUserCommand(
            [FromServices] IValidator<CreateUserRequest> validator,
            [FromServices] IUserRepository repository,
            [FromServices] IMapper<CreateUserRequest, DbUser> mapper)
        {
            this.validator = validator;
            this.repository = repository;
            this.mapper = mapper;
        }

        public Guid Execute(CreateUserRequest request)
        {
            validator.ValidateAndThrow(request);

            return repository.CreateUser(mapper.Map(request));
        }
    }
}
