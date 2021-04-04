namespace LT.DigitalOffice.UserService.Business.Commands.Password.Interfaces
{
    /// <summary>
    /// Represents interface for a command in command pattern.
    /// Provides method for generating password.
    /// </summary>
    public interface IGeneratePasswordCommand
    {
        /// <summary>
        /// Returns randomly generated password string.
        /// </summary>
        string Execute();
    }
}
