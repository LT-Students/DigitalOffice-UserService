using FluentValidation.TestHelper;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Credentials;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Certificates;
using LT.DigitalOffice.UserService.Validation.Certificates;
using LT.DigitalOffice.UserService.Validation.Credentials;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using System;

namespace LT.DigitalOffice.UserService.Validation.UnitTests.Credentials
{
    class CreateCredentialsRequestValidatorTests
    {
        private CreateCredentialsRequestValidator _validator;
        private CreateCredentialsRequest _request;

        [SetUp]
        public void SetUp()
        {
            _validator = new CreateCredentialsRequestValidator();

            _request = new CreateCredentialsRequest
            {
                Login = "Login1234567890",
                Password = "Password",
                UserId = new Guid("A7249129-B47F-490C-8588-C998A76584AB")
            };
        }

        [Test]
        public void ShouldNotThrowAnyException()
        {
            _validator.TestValidate(_request).ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public void ShouldThrowExceptionWhenAllIsNotCorrect()
        {
            _request = new CreateCredentialsRequest
            {
                Login = string.Empty,
                Password = string.Empty,
                UserId = Guid.Empty
            };

            _validator.TestValidate(_request).ShouldHaveValidationErrorFor(x => x.Login);
            _validator.TestValidate(_request).ShouldHaveValidationErrorFor(x => x.Password);
            _validator.TestValidate(_request).ShouldHaveValidationErrorFor(x => x.UserId);
        }

        [Test]
        public void ShouldThrowExceptionWhenLoginTooShort()
        {
            _request = new CreateCredentialsRequest
            {
                Login = new string('a', 2)
            };

            _validator.TestValidate(_request).ShouldHaveValidationErrorFor(x => x.Login);
        }

        [Test]
        public void ShouldThrowExceptionWhenLoginTooLong()
        {
            _request = new CreateCredentialsRequest
            {
                Login = new string('a', 16)
            };

            _validator.TestValidate(_request).ShouldHaveValidationErrorFor(x => x.Login);
        }

        [Test]
        public void ShouldThrowExceptionWhenLoginStartNotInLetter()
        {
            _request = new CreateCredentialsRequest
            {
                Login = new string("1Login")
            };

            _validator.TestValidate(_request).ShouldHaveValidationErrorFor(x => x.Login);
        }

        [Test]
        public void ShouldThrowExceptionWhenLoginHaveSpecialCharacters()
        {
            _request = new CreateCredentialsRequest
            {
                Login = new string("Logi:n")
            };

            _validator.TestValidate(_request).ShouldHaveValidationErrorFor(x => x.Login);
        }
    }
}
