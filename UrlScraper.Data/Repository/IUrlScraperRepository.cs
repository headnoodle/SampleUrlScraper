using System.Collections.Generic;
using UrlScraper.Data.Models;

namespace UrlScraper.Data.Repository
{
    public interface IUrlScraperRepository
    {
        IEnumerable<ScrapeRequest> GetAllRequests();
        int AddNewScraperRequest(string urlToScrape);
        ScrapeResult GetScraperResultForRequestId(int requestId);
        bool AddScrapeResult(int requestId, string data);
    }
}