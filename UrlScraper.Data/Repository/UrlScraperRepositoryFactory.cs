using System;

namespace UrlScraper.Data.Repository
{
    public class UrlScraperRepositoryFactory : IUrlScraperRepositoryFactory
    {
        private readonly string _connectionstring;

        public UrlScraperRepositoryFactory(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            _connectionstring = connectionString;
        }

        public IUrlScraperRepository Create()
        {
            return new UrlScraperRepository(new ScraperDbContext(_connectionstring));
        }
    }
}