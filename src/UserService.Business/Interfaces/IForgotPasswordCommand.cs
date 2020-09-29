using LT.DigitalOffice.UserService.Models.Dto;

namespace LT.DigitalOffice.UserService.Business.Interfaces
{
    /// <summary>
    /// Represents interface for a command in command pattern.
    /// </summary>
    public interface IForgotPasswordCommand
    {
        /// <summary>
        /// Method for getting user description by email and sent request in Message Service
        /// </summary>
        /// <param name="userEmail">Specific user email</param>
        void Execute(ForgotPasswordRequest userEmail);
    }
}
