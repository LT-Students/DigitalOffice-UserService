using FluentValidation;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Interfaces;
using Microsoft.AspNetCore.Mvc;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using System.Linq;

namespace LT.DigitalOffice.UserService.Business
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
            var validationResult = validator.Validate(request);

            if (validationResult != null && !validationResult.IsValid)
            {
                var messages = validationResult.Errors.Select(x => x.ErrorMessage);
                string message = messages.Aggregate((x, y) => x + "\n" + y);

                throw new ValidationException(message);
            }
            return repository.EditUser(mapper.Map(request));
        }
    }
}
