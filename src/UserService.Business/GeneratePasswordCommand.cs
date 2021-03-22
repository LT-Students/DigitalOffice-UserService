using LT.DigitalOffice.UserService.Business.Interfaces;

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
