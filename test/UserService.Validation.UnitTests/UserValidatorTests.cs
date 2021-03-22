using FluentValidation;
using FluentValidation.TestHelper;
using LT.DigitalOffice.UserService.Models.Dto;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LT.DigitalOffice.UserService.Validation.UnitTests
{
    public class UserValidatorTests
    {
        private IValidator<UserRequest> validator;
        private static IEnumerable<Expression<Func<UserRequest, string>>> NamePropertyCases
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
            validator = new UserValidator();
        }

        [TestCaseSource(nameof(NamePropertyCases))]
        public void ShouldThrowValidationExceptionWhenNameIsEmpty(
            Expression<Func<UserRequest, string>> gettingNamePropertyExpression)
        {
            validator.ShouldHaveValidationErrorFor(gettingNamePropertyExpression, "");
        }

        [TestCaseSource(nameof(NamePropertyCases))]
        public void ShouldHaveValidationErrorWhenNameIsTooShort(
            Expression<Func<UserRequest, string>> gettingNamePropertyExpression)
        {
            validator.ShouldHaveValidationErrorFor(gettingNamePropertyExpression, "a");
        }

        [TestCaseSource(nameof(NamePropertyCases))]
        public void ShouldHaveValidationErrorWhenNameIsTooLong(
            Expression<Func<UserRequest, string>> gettingNamePropertyExpression)
        {
            validator.ShouldHaveValidationErrorFor(gettingNamePropertyExpression, new string('a', 100));
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenNameDoesNotMatchRegularExpression(
            [ValueSource(nameof(NamePropertyCases))] Expression<Func<UserRequest, string>> gettingNamePropertyExpression,
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
            var request = new UserRequest();

            validator.TestValidate(request).ShouldHaveAnyValidationError();
        }

        [Test]
        public void ShouldNotThrowValidationExceptionWhenDataIsValid()
        {
            var request = new UserRequest
            {
                Id = Guid.NewGuid(),
                FirstName = "Example",
                LastName = "Example",
                Login = "admin",
                MiddleName = "Example",
                Email = "Example@gmail.com",
                Status = "Example",
                Password = "Example",
                IsAdmin = false,
                Skills = new List<string>()
            };

            validator.TestValidate(request).ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public void ShouldNotThrowValidationExceptionWhenDataIsValidAndIdIsNull()
        {
            var request = new UserRequest
            {
                FirstName = "Example",
                LastName = "Example",
                MiddleName = "Example",
                Login = "admin",
                Email = "Example@gmail.com",
                Status = "Example",
                Password = "Example",
                IsAdmin = false,
                Skills = new List<string>()
            };

            validator.TestValidate(request).ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public void ShouldNotThrowValidationExceptionWhenDataIsValidAndIdIsExist()
        {
            var request = new UserRequest
            {
                Id = Guid.NewGuid(),
                FirstName = "Example",
                LastName = "Example",
                MiddleName = "Example",
                Login = "admin",
                Email = "Example@gmail.com",
                Status = "Example",
                Password = "Example",
                IsAdmin = false
            };

            validator.TestValidate(request).ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenIdUserIsNotValid()
        {
            var request = new UserRequest
            {
                Id = Guid.Empty,
                FirstName = "Example",
                LastName = "Example",
                MiddleName = "Example",
                Email = "Example@gmail.com",
                Status = "Example",
                Password = "Example",
                IsAdmin = false
            };

            validator.TestValidate(request).ShouldHaveValidationErrorFor(x => x.Id.Value);
        }

        [Test]
        public void ShouldNotThrowValidationExceptionWhenDataIsValidAndContainsRussianSymbols()
        {
            var request = new UserRequest
            {
                Id = Guid.NewGuid(),
                FirstName = "Пример",
                LastName = "Пример",
                MiddleName = "Пример",
                Login = "админ",
                Email = "Example@gmail.com",
                Status = "Example",
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
            var connections = new List<UserConnection>();

            validator.ShouldNotHaveValidationErrorFor(x => x.Connections, connections);
        }

        [Test]
        public void ShouldPassWhenDataIsValidWithRightConnections()
        {
            var connections = new List<UserConnection>()
                {
                    new UserConnection()
                    {
                        Type = ConnectionType.Email,
                        Value = "Ex@mail.ru"
                    }
                };

            validator.ShouldNotHaveValidationErrorFor(x => x.Connections, connections);
        }

        [Test]
        public void ShouldPassWhenDataIsValidWithWrongConnections()
        {
            var request = new UserRequest
            {
                Id = Guid.NewGuid(),
                FirstName = "Example",
                LastName = "Example",
                Email = "Example@gmail.com",
                Status = "Example",
                Password = "Example",
                IsAdmin = false,
                Connections = new List<UserConnection>()
                {
                    new UserConnection()
                    {
                        Type = ConnectionType.Email,
                        Value = ""
                    }
                },
                Login = "Example"
            };

            validator.TestValidate(request).ShouldHaveAnyValidationError();
        }
    }
}
