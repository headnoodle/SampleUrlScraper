using Microsoft.Extensions.Logging;
using UrlScraper.Shared.Options;

namespace UrlScraper.Shared
{
    public class SqsQueueFactory
    {
        private readonly ILogger<SqsQueue> _logger;

        public SqsQueueFactory(ILogger<SqsQueue> logger)
        {
            _logger = logger;
        }

        public ISqsQueue CreateQueue(SqsConfigurator configuration)
        {
            return new SqsQueue(_logger, configuration);
        }
    }
}