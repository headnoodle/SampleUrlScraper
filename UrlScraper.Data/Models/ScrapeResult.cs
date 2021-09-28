using System.ComponentModel.DataAnnotations;

namespace UrlScraper.Data.Models
{
    public class ScrapeResult
    {
        [Key] 
        public int ScrapeResultId { get; set; }
        public int ScrapeRequestId { get; set; }
        public string ResultData { get; set; }
    }
}