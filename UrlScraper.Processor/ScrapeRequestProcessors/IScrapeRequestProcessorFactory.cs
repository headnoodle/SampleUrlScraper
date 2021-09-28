namespace UrlScraper.Processor.ScrapeRequestProcessors
{
    public interface IScrapeRequestProcessorFactory
    {
        IScrapeRequestProcessor CreateProcessor();
    }
}