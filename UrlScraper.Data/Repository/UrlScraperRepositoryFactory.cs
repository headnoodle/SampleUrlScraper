namespace UrlScraper.Data.Repository
{
    public class UrlScraperRepositoryFactory : IUrlScraperRepositoryFactory
    {
        private readonly string _connectionstring;

        public UrlScraperRepositoryFactory(string connectionstring)
        {
            _connectionstring = connectionstring;
        }

        public IUrlScraperRepository Create()
        {
            return new UrlScraperRepository(new ScraperDbContext(_connectionstring));
            //return new UrlScraperRepository(new ScraperDbContext());
        }
    }
}