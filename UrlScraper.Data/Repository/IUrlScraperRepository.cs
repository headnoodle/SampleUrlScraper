using System;
using System.Collections.Generic;
using UrlScraper.Data.Models;

namespace UrlScraper.Data.Repository
{
    public interface IUrlScraperRepository
    {
        IEnumerable<ScrapeRequest> GetAllRequests();
        Guid AddNewScraperRequest(string urlToScrape);
        ScrapeResult GetScraperResultForToken(Guid token);
        bool AddScrapeResult(Guid token, string data);
    }
}