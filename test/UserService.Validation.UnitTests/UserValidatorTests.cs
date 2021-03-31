using FluentValidation;
using FluentValidation.TestHelper;
using LT.DigitalOffice.UserService.Models.Dto;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LT.DigitalOffice.UserService.Validation.UnitTests
{
    public class UserValidatorTests
    {
        private IValidator<CreateUserRequest> validator;
        private static IEnumerable<Expression<Func<CreateUserRequest, string>>> NamePropertyCases
        {
            get
            {
                yield return request => request.FirstName;
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
                yield return "Приmer";
                yield return "ПРимер";
                yield return "примеР";
            }
        }

        [SetUp]
        public void SetUp()
        {
            validator = new CreateUserRequestValidator();
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
        public void ShouldThrowValidationExceptionWhenStatusNull()
        {
            var status = (UserStatus)5;
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
                Login = "admin",
                MiddleName = "Example",
                Status = UserStatus.Sick,
                Password = "Example",
                IsAdmin = false,
                Skills = new List<string>()
            };

            validator.TestValidate(request).ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public void ShouldNotThrowValidationExceptionWhenDataIsValidAndIdIsNull()
        {
            var request = new CreateUserRequest
            {
                FirstName = "Example",
                LastName = "Example",
                MiddleName = "Example",
                Login = "admin",
                Status = UserStatus.Sick,
                Password = "Example",
                IsAdmin = false,
                Skills = new List<string>()
            };

            validator.TestValidate(request).ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public void ShouldNotThrowValidationExceptionWhenDataIsValidAndIdIsExist()
        {
            var request = new CreateUserRequest
            {
                FirstName = "Example",
                LastName = "Example",
                MiddleName = "Example",
                Login = "admin",
                Status = UserStatus.Sick,
                Password = "Example",
                IsAdmin = false
            };

            validator.TestValidate(request).ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public void ShouldNotThrowValidationExceptionWhenDataIsValidAndContainsRussianSymbols()
        {
            var request = new CreateUserRequest
            {
                FirstName = "Пример",
                LastName = "Пример",
                MiddleName = "Пример",
                Login = "админ",
                Status = UserStatus.Sick,
                Password = "Example",
                IsAdmin = false
            };

            validator.TestValidate(request).ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public void ShouldPassWhenDataIsValidWithoutMiddleName()
        {
            string middleName = null;

            validator.ShouldNotHaveValidationErrorFor(x => x.MiddleName, middleName);
        }

        [Test]
        public void ShouldPassWhenSkillsIsNull()
        {
            List<string> skills = null;

            validator.ShouldNotHaveValidationErrorFor(x => x.Skills, skills);
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenSkillNameIsTooLong()
        {
            var skills = new List<string>() { "C#", "some very looooooooooong name skill" };

            validator.ShouldHaveValidationErrorFor(x => x.Skills, skills);
        }

        [Test]
        public void ShouldPassWhenDataIsValidWithEmptyConnections()
        {
            var request = new CreateUserRequest
            {
                FirstName = "Example",
                LastName = "Example",
                Status = UserStatus.Sick,
                Password = "Example",
                IsAdmin = false,
                Communications = new List<Communications>(),
                Login = "Example"
            };

            validator.TestValidate(request).ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public void ShouldPassWhenDataIsValidWithRightConnections()
        {
            var request = new CreateUserRequest
            {
                FirstName = "Example",
                LastName = "Example",
                Status = UserStatus.Sick,
                Password = "Example",
                IsAdmin = false,
                Communications = new List<Communications>()
                {
                    new Communications()
                    {
                        Type = CommunicationType.Email,
                        Value = "Ex@mail.ru"
                    }
                },
                Login = "Example"
            };

            validator.TestValidate(request).ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public void ShouldPassWhenDataIsValidWithWrongConnections()
        {
            var request = new CreateUserRequest
            {
                FirstName = "Example",
                LastName = "Example",
                Status = UserStatus.Sick,
                Password = "Example",
                IsAdmin = false,
                Communications = new List<Communications>()
                {
                    new Communications()
                    {
                        Type = CommunicationType.Email,
                        Value = ""
                    }
                },
                Login = "Example"
            };

            validator.TestValidate(request).ShouldHaveAnyValidationError();
        }
    }
}
