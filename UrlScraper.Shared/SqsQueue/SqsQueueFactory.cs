using System;
using Microsoft.Extensions.Logging;
using UrlScraper.Shared.Options;

namespace UrlScraper.Shared.SqsQueue
{
    public class SqsQueueFactory
    {
        private readonly ILogger<SqsQueue> _logger;

        public SqsQueueFactory(ILogger<SqsQueue> logger)
        {
            _logger = logger??throw new ArgumentNullException(nameof(logger));
        }

        public ISqsQueue CreateQueue(SqsConfigurator configuration)
        {
            return new SqsQueue(_logger, configuration);
        }
    }
}