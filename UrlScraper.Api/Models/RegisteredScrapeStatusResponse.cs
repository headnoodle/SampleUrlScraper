using System;

namespace UrlScraper.Api.Models
{
    public class RegisteredScrapeStatusResponse
    {
        public Guid Token { get; set; }  
        public string Url { get; set; }
        public bool Processed { get; set; }
    }
}