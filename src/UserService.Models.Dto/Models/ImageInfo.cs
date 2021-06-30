using System;

namespace LT.DigitalOffice.UserService.Models.Dto.Models
{
    public record ImageInfo
    {
        public Guid? Id { get; set; }
        public Guid? ParentId { get; set; }
        public string Content { get; set; }
        public string Extension { get; set; }
    }
}
