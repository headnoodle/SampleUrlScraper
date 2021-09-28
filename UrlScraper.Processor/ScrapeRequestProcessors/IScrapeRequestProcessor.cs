using System.Threading;
using System.Threading.Tasks;
using UrlScraper.Shared.Models;

namespace UrlScraper.Processor.ScrapeRequestProcessors
{
    public interface IScrapeRequestProcessor
    {
        Task<QueueUrlScrapeResult> ProcessScrapeRequestAsync(QueueUrlScrapeRequest queueUrlScrapeRequest, CancellationToken cancellationToken);
    }
}