using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using UrlScraper.Data.Models;

namespace UrlScraper.Data.Repository
{
    public class UrlScraperRepository : IUrlScraperRepository
    {
        private readonly ScraperDbContext _dbContext;


        public UrlScraperRepository(ScraperDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IEnumerable<ScrapeRequest> GetAllRequests()
        {
            return (from request in _dbContext.ScrapeRequests
                    select request).ToList();
        }

        public int AddNewScraperRequest(string urlToScrape)
        {

            var scrapeRequest = new ScrapeRequest() { Url = urlToScrape};
            _dbContext.Add(scrapeRequest);
            _dbContext.SaveChanges();
            return scrapeRequest.ScrapeRequestId;

        }

        public ScrapeResult GetScraperResultForRequestId(int requestId)
        {
            return (from result in _dbContext.ScrapeRequestResults
                    where result.ScrapeRequestId == requestId
                    select result)
                .FirstOrDefault();
        }

        public bool AddScrapeResult(int requestId, string data)
        {

            var scrapeRequest = (
                from request in _dbContext.ScrapeRequests
                where request.ScrapeRequestId == requestId
                select request)
                .Include(b => b.ScrapeResult)
                .FirstOrDefault();

            if (scrapeRequest == null)
                return false;

            scrapeRequest.Processed = true;

            if (scrapeRequest.ScrapeResult != null) 
                scrapeRequest.ScrapeResult.ResultData = data;
            else
                scrapeRequest.ScrapeResult = new ScrapeResult() { ResultData = data };

            return (_dbContext.SaveChanges() >0);
        }
    }
}