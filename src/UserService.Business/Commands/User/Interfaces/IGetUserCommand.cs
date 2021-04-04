using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Filters;
using LT.DigitalOffice.UserService.Models.Dto.Responses.User;

namespace LT.DigitalOffice.UserService.Business.Interfaces
{
    /// <summary>
    /// Represents interface for a command in command pattern.
    /// Provides method for getting user information.
    /// </summary>
    public interface IGetUserCommand
    {
        /// <summary>
        /// Returns the user information.
        /// </summary>
        UserResponse Execute(GetUserFilter filter);
    }
}