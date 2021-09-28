using UrlScraper.Shared.Options;

namespace UrlScraper.Api.Options
{
    public class QueueOptions
    {
        public SqsConfigurator RequestOptions { get; set; }
        public SqsConfigurator ResultOptions { get; set; }
    }
}