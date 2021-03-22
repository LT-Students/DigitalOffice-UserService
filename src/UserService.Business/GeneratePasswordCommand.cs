using LT.DigitalOffice.UserService.Business.Interfaces;
using System;
using System.Linq;

namespace LT.DigitalOffice.UserService.Business
{
    public class GeneratePasswordCommand : IGeneratePasswordCommand
    {
        public string Execute()
        {
            return PasswordGenerationLogic.GeneratePassword();
        }
    }
}
