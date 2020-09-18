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
            var validationResult = validator.Validate(userEmail);

            if (validationResult != null && !validationResult.IsValid)
            {
                var messages = validationResult.Errors.Select(x => x.ErrorMessage);
                string message = messages.Aggregate((x, y) => x + "\n" + y);

                throw new ValidationException(message);
            }

            return mapper.Map(repository.GetUserByEmail(userEmail));
        }
    }
}
