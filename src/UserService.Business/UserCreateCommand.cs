using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System;
using UserService.Business.Interfaces;
using UserService.Data.Interfaces;
using UserService.Mappers.Interfaces;
using UserService.Models.Db;
using UserService.Models.Dto;

namespace UserService.Business
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
