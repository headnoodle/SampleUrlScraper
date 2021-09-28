namespace UrlScraper.Shared.Options
{
    public class SqsConfigurator
    {
        public string QueueUrl { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string Region { get; set; }
    }
}