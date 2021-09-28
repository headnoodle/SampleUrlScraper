namespace UrlScraper.Data.Repository
{
    public interface IUrlScraperRepositoryFactory
    {
        IUrlScraperRepository Create();
    }
}