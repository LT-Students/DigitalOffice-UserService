namespace LT.DigitalOffice.UserService.Models.Dto.Configurations
{
    public class CacheConfig
    {
        public const string SectionName = "MemoryCache";

        public double CacheLiveInMinutes { get; set; }
    }
}
