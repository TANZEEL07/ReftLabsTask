namespace ReftLabsTask.Internal.Infrastructure.Configuration
{
    public class ReqResApiOptions
    {
        public string BaseUrl { get; set; } = string.Empty;
        public int CacheDurationSeconds { get; set; } = 300;
        public string ApiKey { get; set; }
    }
}
