using System;
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

        public Guid AddNewScraperRequest(string urlToScrape)
        {

            var scrapeRequest = new ScrapeRequest() { Url = urlToScrape, Token = Guid.NewGuid()};
            _dbContext.Add(scrapeRequest);
            _dbContext.SaveChanges();
            return scrapeRequest.Token;

        }

        public ScrapeResult GetScraperResultForToken(Guid token)
        {
            return (from request in _dbContext.ScrapeRequests
                    where request.Token== token 
                    select request.ScrapeResult)
                .FirstOrDefault();
        }

        public bool AddScrapeResult(Guid token, string data)
        {

            var scrapeRequest = (
                from request in _dbContext.ScrapeRequests
                where request.Token == token
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