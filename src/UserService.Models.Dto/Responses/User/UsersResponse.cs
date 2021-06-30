using LT.DigitalOffice.UserService.Models.Dto.Models;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Models.Dto.Responses.User
{
    public record UsersResponse
    {
        public int TotalCount { get; set; }
        public List<UserInfo> Users { get; set; } = new();
        public List<string> Errors { get; set; } = new();
    }
}
