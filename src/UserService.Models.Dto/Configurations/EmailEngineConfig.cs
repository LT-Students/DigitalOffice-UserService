namespace LT.DigitalOffice.UserService.Models.Dto.Configurations
{
    public record EmailEngineConfig
    {
        public const string SectionName = "EmailEngineConfig";

        public int ResendIntervalInMinutes { get; set; }
    }
}
