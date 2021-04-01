using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.ResponsesMappers.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto;
using LT.DigitalOffice.UserService.Validation.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.UserService.Business
{
    /// <inheritdoc/>
    public class GetUserByEmailCommand : IGetUserByEmailCommand
    {
        private readonly IEmailValidator validator;
        private readonly IUserRepository repository;
        private readonly IUserResponseMapper mapper;
        ILogger<GetUserByEmailCommand> _logger;

        public GetUserByEmailCommand(
            [FromServices] IEmailValidator validator,
            [FromServices] IUserRepository repository,
            [FromServices] IUserResponseMapper mapper,
            ILogger<GetUserByEmailCommand> logger)
        {
            this.validator = validator;
            this.repository = repository;
            this.mapper = mapper;
            _logger = logger;
        }

        /// <inheritdoc/>
        public User Execute(string userEmail)
        {

            _logger.LogInformation("sjdncjscdnsjcjsnc");

            validator.ValidateAndThrowCustom(userEmail);

            var user = mapper.Map(repository.GetUserByEmail(userEmail));

            return user;
        }
    }
}