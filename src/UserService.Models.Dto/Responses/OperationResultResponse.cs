using LT.DigitalOffice.UserService.Models.Dto.Enums;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Models.Dto.Responses
{
    public record OperationResultResponse<T>
    {
        public T Body { get; set; }
        public OperationResultStatusType Status { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
