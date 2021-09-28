using System;
using System.Threading;
using System.Threading.Tasks;
using UrlScraper.Shared.Models;

namespace UrlScraper.Processor.ScrapeRequestProcessors
{
    public class ScrapeRequestProcessor : IScrapeRequestProcessor
    {
        public async Task<QueueUrlScrapeResult> ProcessScrapeRequestAsync(QueueUrlScrapeRequest queueUrlScrapeRequest, CancellationToken cancellationToken)
        {

            int waitTime = new Random().Next(30, 60);
            await Task.Delay(TimeSpan.FromSeconds(waitTime), cancellationToken);

            string words = LoremNET.Lorem.Paragraph(20, 40, 5, 10);
            var returnData = $"<html><body>{words}</body></html>";

            return new QueueUrlScrapeResult() {data = returnData, Token = queueUrlScrapeRequest.Token};
        }


    }
}