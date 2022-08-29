using LT.DigitalOffice.UserService.Business.Commands.Password;
using LT.DigitalOffice.UserService.Business.Commands.Password.Interfaces;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace LT.DigitalOffice.UserService.Business.UnitTests.Password
{
  public class GeneratePasswordCommandTests
  {
    private IGeneratePasswordCommand command;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
      command = new GeneratePasswordCommand();
    }

    [Test]
    public void ShouldCheckWhetherThePasswordGeneratingCorrectly()
    {
      for (int i = 0; i < 250; i++)
      {
        Assert.IsTrue(Regex.IsMatch(command.Execute(), "^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@!$_*#]).{8,12}$"));
      }
    }
  }
}
