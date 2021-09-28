namespace UrlScraper.Processor.ScrapeRequestProcessors
{
    public class ScrapeRequestProcessorFactory : IScrapeRequestProcessorFactory
    {
        public IScrapeRequestProcessor CreateProcessor()
        {
            return new ScrapeRequestProcessor();
        }
    }
}