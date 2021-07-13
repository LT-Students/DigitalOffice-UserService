using FluentValidation.TestHelper;
using LT.DigitalOffice.UserService.Models.Dto;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using LT.DigitalOffice.UserService.Validation.User;
using LT.DigitalOffice.UserService.Validation.User.Interfaces;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LT.DigitalOffice.UserService.Validation.UnitTests
{
    public class UserValidatorTests
    {
        private ICreateUserRequestValidator validator;

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

        /*[TestCaseSource(nameof(NamePropertyCases))]
        public void ShouldThrowValidationExceptionWhenNameIsEmpty(
            Expression<Func<CreateUserRequest, string>> gettingNamePropertyExpression)
        {
            validator.ShouldHaveValidationErrorFor(gettingNamePropertyExpression, "");
        }

        [TestCaseSource(nameof(NamePropertyCases))]
        public void ShouldHaveValidationErrorWhenNameIsTooLong(
            Expression<Func<CreateUserRequest, string>> gettingNamePropertyExpression)
        {
            validator.ShouldHaveValidationErrorFor(gettingNamePropertyExpression, new string('a', 100));
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenStatusNull()
        {
            var status = (UserStatus)5;
            validator.ShouldHaveValidationErrorFor(x => x.Status, status);
        }

        [Test]
        public void ShouldThrowExceptionWhenGenderIsNull()
        {
            var gender = (UserGender)5;
            validator.ShouldHaveValidationErrorFor(x => x.Gender, gender);
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenAllFieldsAreEmpty()
        {
            var request = new CreateUserRequest();

            validator.TestValidate(request).ShouldHaveAnyValidationError();
        }*/

        [Test]
        public void ShouldNotThrowValidationExceptionWhenDataIsValid()
        {
            var request = new CreateUserRequest
            {
                FirstName = "Example",
                LastName = "Example",
                MiddleName = "Example",
                Gender = UserGender.NotSelected,
                City = "Spb",
                Status = UserStatus.Sick,
                IsAdmin = false,
                Rate = 0.25
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
                Gender = UserGender.NotSelected,
                City = "Spb",
                Status = UserStatus.Sick,
                IsAdmin = false,
                Rate = 0.25
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
                Gender = UserGender.NotSelected,
                City = "Spb",
                Status = UserStatus.Sick,
                IsAdmin = false,
                Rate = 0.25
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
                Gender = UserGender.NotSelected,
                City = "Spb",
                Status = UserStatus.Sick,
                IsAdmin = false,
                Rate = 0.25
            };

            validator.TestValidate(request).ShouldNotHaveAnyValidationErrors();
        }

        /*[Test]
        public void ShouldPassWhenDataIsValidWithoutMiddleName()
        {
            string middleName = null;

            validator.ShouldNotHaveValidationErrorFor(x => x.MiddleName, middleName);
        }*/

        [Test]
        public void ShouldPassWhenDataIsValidWithEmptyConnections()
        {
            var request = new CreateUserRequest
            {
                FirstName = "Example",
                LastName = "Example",
                Gender = UserGender.NotSelected,
                City = "Spb",
                Status = UserStatus.Sick,
                IsAdmin = false,
                Rate = 0.25,
                Communications = new List<CommunicationInfo>()
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
                Gender = UserGender.NotSelected,
                City = "Spb",
                Status = UserStatus.Sick,
                IsAdmin = false,
                Rate = 0.25,
                Communications = new List<CommunicationInfo>()
                {
                    new CommunicationInfo()
                    {
                        Type = CommunicationType.Email,
                        Value = "Ex@mail.ru"
                    }
                }
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
                Gender = UserGender.NotSelected,
                City = "Spb",
                Status = UserStatus.Sick,
                IsAdmin = false,
                Communications = new List<CommunicationInfo>()
                {
                    new CommunicationInfo()
                    {
                        Type = CommunicationType.Email,
                        Value = ""
                    }
                }
            };

            validator.TestValidate(request).ShouldHaveAnyValidationError();
        }
    }
}
