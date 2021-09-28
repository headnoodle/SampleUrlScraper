using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UrlScraper.Processor.ScrapeRequestProcessors;
using UrlScraper.Shared.Models;
using UrlScraper.Shared.SqsQueue;

namespace UrlScraper.Processor.BackgroundServices
{
    public class UrlScraperService : BackgroundService
    {
        private readonly ILogger<UrlScraperService> _logger;
        private readonly ISqsQueue _scraperRequestQueue;
        private readonly ISqsQueue _scraperResultsQueue;
        private readonly IScrapeRequestProcessorFactory _scrapeRequestProcessorFactory;

        public UrlScraperService(ILogger<UrlScraperService> logger, ISqsQueue scraperRequestQueue, ISqsQueue scraperResultsQueue, IScrapeRequestProcessorFactory scrapeRequestProcessorFactory)
        {
            _logger = logger??throw new ArgumentNullException(nameof(logger));
            _scraperRequestQueue = scraperRequestQueue?? throw new ArgumentNullException(nameof(scraperRequestQueue));
            _scraperResultsQueue = scraperResultsQueue?? throw new ArgumentNullException(nameof(scraperResultsQueue));
            _scrapeRequestProcessorFactory = scrapeRequestProcessorFactory??throw new ArgumentNullException(nameof(scrapeRequestProcessorFactory));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var response = await _scraperRequestQueue.ReceiveMessageResponse(1, 20, stoppingToken);

                    if (isMessageResponseSuccessful(response))
                    {
                        foreach (var message in response.Messages)
                        {
                            try
                            {
                                var request = JsonSerializer.Deserialize<QueueUrlScrapeRequest>(message.Body);
                                LogPickupMessage(message);
                                var result = await ProcessScrapeRequest(request, stoppingToken);
                                var resultJson = JsonSerializer.Serialize(result);

                                if (await AddResultsToResponseQueue(resultJson, stoppingToken))
                                    await _scraperRequestQueue.DeleteMessageAsync(message, stoppingToken);
                            }
                            catch (Exception e)
                            {
                                _logger.LogError(e, "Problem processing message from queue");
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Problem getting message from queue");
                }

                


            }
        }

        private async Task<bool> AddResultsToResponseQueue(string results, CancellationToken cancellationToken)
        {
            return await _scraperResultsQueue.SendMessageAsync(results, cancellationToken);
        }

        private void LogPickupMessage(Message message)
        {
            _logger.LogInformation($"Message picked up with ID: {message.MessageId} and Body:{message.Body}");
        }

        private async Task<QueueUrlScrapeResult> ProcessScrapeRequest(QueueUrlScrapeRequest scrapeRequest, CancellationToken cancellationToken)
        {
            var scrapeRequestProcessor = _scrapeRequestProcessorFactory.CreateProcessor();

            _logger.LogInformation($"Processing Scrape Request Token: {scrapeRequest.Token}");

            var timer = Stopwatch.StartNew();
            var scrapeResult = await scrapeRequestProcessor.ProcessScrapeRequestAsync(scrapeRequest, cancellationToken);
            timer.Stop();
            
            _logger.LogInformation($"Processed Scrape Request and it took {timer.Elapsed.Seconds} seconds to run");

            return scrapeResult;

        }

        private bool isMessageResponseSuccessful(ReceiveMessageResponse response)
        {
            if (response == null)
                return false;

            return response.HttpStatusCode == HttpStatusCode.OK && response.Messages.Any();
        }
    }
}
