using System;

namespace UrlScraper.Shared.Models
{
    public class QueueUrlScrapeRequest
    {
        public Guid Token { get; set; }
        public string Url { get; set; }
    }
}