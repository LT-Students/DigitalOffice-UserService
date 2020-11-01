using FluentValidation;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.ResponsesMappers.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.UserService.Business
{
    /// <inheritdoc/>
    public class GetUserByEmailCommand : IGetUserByEmailCommand
    {
        private readonly IValidator<string> validator;
        private readonly IUserRepository repository;
        private readonly IUserResponseMapper mapper;

        public GetUserByEmailCommand(
            [FromServices] IValidator<string> validator,
            [FromServices] IUserRepository repository,
            [FromServices] IUserResponseMapper mapper)
        {
            this.validator = validator;
            this.repository = repository;
            this.mapper = mapper;
        }

        /// <inheritdoc/>
        public User Execute(string userEmail)
        {
            validator.ValidateAndThrowCustom(userEmail);

            var user = mapper.Map(repository.GetUserByEmail(userEmail));

            return user;
        }
    }
}