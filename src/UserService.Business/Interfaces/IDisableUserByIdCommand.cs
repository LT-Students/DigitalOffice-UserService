using System;

namespace LT.DigitalOffice.UserService.Business.Interfaces
{
    /// <summary>
    /// Represents interface for a command in command pattern.
    /// Provides method for getting result of operation to disabling user.
    /// </summary>
    public interface IDisableUserByIdCommand
    {
        /// <summary>
        /// The result of operation with boolean value.
        /// </summary>
        /// <param name="userId">Specified id.</param>
        void Execute(Guid userId);
    }
}
