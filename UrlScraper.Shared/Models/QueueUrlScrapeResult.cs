using System;

namespace UrlScraper.Shared.Models
{
    public class QueueUrlScrapeResult
    {
        public Guid Token { get; set; }
        public string  data { get; set; }
    }
}