namespace LT.DigitalOffice.UserService.Models.Dto.Configurations
{
    public record CacheConfig
    {
        public const string SectionName = "MemoryCache";

        public double CacheLiveInMinutes { get; set; }
    }
}
