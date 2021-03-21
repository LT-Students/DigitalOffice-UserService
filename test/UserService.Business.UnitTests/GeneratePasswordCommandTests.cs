using LT.DigitalOffice.UserService.Business.Interfaces;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.UnitTests
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
