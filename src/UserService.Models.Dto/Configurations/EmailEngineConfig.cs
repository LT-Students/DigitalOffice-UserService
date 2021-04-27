namespace LT.DigitalOffice.UserService.Models.Dto.Configurations
{
    public class EmailEngineConfig
    {
        public const string SectionName = "EmailEngineConfig";

        public int ResendIntervalInMinutes { get; set; }
    }
}
