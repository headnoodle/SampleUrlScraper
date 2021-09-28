using System;
using System.ComponentModel.DataAnnotations;

namespace UrlScraper.Data.Models
{
    public class ScrapeRequest
    {
        [Key] 
        public int ScrapeRequestId { get; set; }
        public Guid Token { get; set; }
        public string Url { get; set; }
        public bool Processed { get; set; }
        public ScrapeResult ScrapeResult { get; set; }
        }
}
