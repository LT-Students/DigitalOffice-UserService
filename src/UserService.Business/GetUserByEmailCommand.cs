using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using UserService.Business.Interfaces;
using UserService.Data.Interfaces;
using UserService.Mappers.Interfaces;
using UserService.Models.Db;
using UserService.Models.Dto;

namespace UserService.Business
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

            var user = mapper.Map(repository.GetUserByEmail(userEmail));

            return user;
        }
    }
}
