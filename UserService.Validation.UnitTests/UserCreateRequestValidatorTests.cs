using FluentValidation;
using FluentValidation.TestHelper;
using LT.DigitalOffice.UserService.Models.Dto;
using LT.DigitalOffice.UserService.Validation;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LT.DigitalOffice.UserService.Validation.UnitTests
{
    public class UserCreateRequestValidatorTests
    {
        private IValidator<CreateUserRequest> validator;
        private static IEnumerable<Expression<Func<CreateUserRequest, string>>> NamePropertyCases
        {
            get
            {
                yield return request => request.FirstName;
                yield return request => request.MiddleName;
                yield return request => request.LastName;
            }
        }

        private static IEnumerable<string> NamesThatDoesNotMatchPatternCases
        {
            get
            {
                yield return "X æ A-12";
                yield return "ExampleW1thNumber!1";
                yield return "examplelowcase";
                yield return "EXAMPLCAPITALLETTER";
            }
        }

        private static IEnumerable<string> WrongEmailCases
        {
            get
            {
                yield return "@.";
                yield return "@gmail.com";
                yield return "@.com";
                yield return "Hello!";
            }
        }

        [SetUp]
        public void SetUp()
        {
            validator = new UserCreateRequestValidator();
        }

        [TestCaseSource(nameof(NamePropertyCases))]
        public void ShouldThrowValidationExceptionWhenNameIsEmpty(
            Expression<Func<CreateUserRequest, string>> gettingNamePropertyExpression)
        {
            validator.ShouldHaveValidationErrorFor(gettingNamePropertyExpression, "");
        }

        [TestCaseSource(nameof(NamePropertyCases))]
        public void ShouldHaveValidationErrorWhenNameIsTooShort(
            Expression<Func<CreateUserRequest, string>> gettingNamePropertyExpression)
        {
            validator.ShouldHaveValidationErrorFor(gettingNamePropertyExpression, "a");
        }

        [TestCaseSource(nameof(NamePropertyCases))]
        public void ShouldHaveValidationErrorWhenNameIsTooLong(
            Expression<Func<CreateUserRequest, string>> gettingNamePropertyExpression)
        {
            validator.ShouldHaveValidationErrorFor(gettingNamePropertyExpression, new string('a', 100));
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenNameDoesNotMatchRegularExpression(
            [ValueSource(nameof(NamePropertyCases))] Expression<Func<CreateUserRequest, string>> gettingNamePropertyExpression,
            [ValueSource(nameof(NamesThatDoesNotMatchPatternCases))]
            string name)
        {
            validator.ShouldHaveValidationErrorFor(gettingNamePropertyExpression, name);
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenEmailIsEmpty()
        {
            validator.ShouldHaveValidationErrorFor(x => x.Email, "");
        }

        [TestCaseSource(nameof(WrongEmailCases))]
        public void ShouldThrowValidationExceptionWhenEmailIsInvalid(string wrongEmail)
        {
            validator.ShouldHaveValidationErrorFor(x => x.Email, wrongEmail);
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenEmailTooLong()
        {
            var email = new string('a', 400) + "@gmail.com";
            validator.ShouldHaveValidationErrorFor(x => x.Email, email);
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenStatusTooLong()
        {
            var status = new string('a', 300) + "@gmail.com";
            validator.ShouldHaveValidationErrorFor(x => x.Status, status);
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenPasswordIsEmpty()
        {
            validator.ShouldHaveValidationErrorFor(x => x.Password, "");
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenAllFieldsAreEmpty()
        {
            var request = new CreateUserRequest();

            validator.TestValidate(request).ShouldHaveAnyValidationError();
        }

        [Test]
        public void ShouldNotThrowValidationExceptionWhenDataIsValid()
        {
            var request = new CreateUserRequest
            {
                FirstName = "Example",
                LastName = "Example",
                MiddleName = "Example",
                Email = "Example@gmail.com",
                Status = "Example",
                Password = "Example",
                IsAdmin = false
            };

            validator.TestValidate(request).ShouldNotHaveAnyValidationErrors();
        }
    }
}
