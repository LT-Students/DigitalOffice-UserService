namespace LT.DigitalOffice.UserService.Configuration
{
    public class LogstashConfig
    {
        public const string LogstashSectionName = "Logstash";

        public string KeyProperty { get; set; }
        public string Url { get; set; }
    }
}
