using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using UserService.Business.Interfaces;
using UserService.Data.Interfaces;
using UserService.Mappers.Interfaces;
using UserService.Models.Db;
using UserService.Models.Dto;

namespace UserService.Business
{
    public class EditUserCommand : IEditUserCommand
    {
        private readonly IValidator<EditUserRequest> validator;
        private readonly IUserRepository repository;
        private readonly IMapper<EditUserRequest, DbUser> mapper;

        public EditUserCommand(
            [FromServices] IValidator<EditUserRequest> validator,
            [FromServices] IUserRepository repository,
            [FromServices] IMapper<EditUserRequest, DbUser> mapper)
        {
            this.validator = validator;
            this.repository = repository;
            this.mapper = mapper;
        }

        public bool Execute(EditUserRequest request)
        {
            validator.ValidateAndThrow(request);

            var user = mapper.Map(request);

            return repository.EditUser(user);
        }
    }
}
