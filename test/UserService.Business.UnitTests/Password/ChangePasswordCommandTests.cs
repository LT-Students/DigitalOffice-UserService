using FluentValidation.Results;
using LT.DigitalOffice.UserService.Validation.Password.Interfaces;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;

namespace LT.DigitalOffice.UserService.Business.UnitTests.Password
{
  internal class ChangePasswordCommandTests
  {
    private AutoMocker _mocker;

    [SetUp]
    public void SerUp()
    {
      _mocker = new AutoMocker();

      _mocker
        .Setup<IPasswordValidator, ValidationResult>(x => x.Validate(It.IsAny<string>()))
        .Returns(new ValidationResult());

      _mocker
        .Setup<IPasswordValidator, ValidationResult>(x => x.Validate(It.IsAny<string>()))
        .Returns(new ValidationResult());
    }
  }
}
