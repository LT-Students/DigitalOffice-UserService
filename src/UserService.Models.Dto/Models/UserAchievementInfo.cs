using LT.DigitalOffice.UserService.Models.Dto.Models;
using System;

namespace LT.DigitalOffice.UserService.Models.Dto
{
    public record UserAchievementInfo
    {
        public Guid Id { get; set; }
        public Guid AchievementId { get; set; }
        public string Name { get; set; }
        public DateTime ReceivedAt { get; set; }
        public ImageConsist Image { get; set; }
    }
}