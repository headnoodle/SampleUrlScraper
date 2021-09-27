using System;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UrlScraper.Data.Repository;
using UrlScraper.Shared.Models;
using UrlScraper.Shared.SqsQueue;

namespace UrlScraper.Api.BackgroundServices
{
    public class UrlScraperResultsProcessor : BackgroundService
    {
        private readonly ILogger<UrlScraperResultsProcessor> _logger;
        private readonly ISqsQueue _resultsQueue;
        private readonly IUrlScraperRepository _scraperRepository;

        public UrlScraperResultsProcessor(ILogger<UrlScraperResultsProcessor> logger, ISqsQueue resultsQueue, IUrlScraperRepositoryFactory scraperRepositoryFactory)
        {
            _logger = logger?? throw new ArgumentNullException(nameof(logger));
            _resultsQueue = resultsQueue?? throw new ArgumentNullException(nameof(resultsQueue));
            _scraperRepository = scraperRepositoryFactory?.Create()?? throw  new ArgumentNullException(nameof(resultsQueue));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var response = await GetMessageFromQueue(stoppingToken);

                    if (isMessageAvailable(response))
                        foreach (var message in response.Messages)
                            await ProcessMessage(stoppingToken, message);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Problem processing result message");
                }
            }
        }

        private async Task<ReceiveMessageResponse> GetMessageFromQueue(CancellationToken stoppingToken)
        {
            return await _resultsQueue.ReceiveMessageResponse(1, 20, stoppingToken);
        }

        private async Task ProcessMessage(CancellationToken stoppingToken, Message message)
        {
            try
            {
                AddMessageToStore(message);
                await DeleteMessageFromQueue(stoppingToken, message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error processesing Result");
            }
        }

        private void AddMessageToStore(Message message)
        {
            var scrapeResult = JsonSerializer.Deserialize<QueueUrlScrapeResult>(message.Body);

            if (scrapeResult != null)
            {
                _logger.LogInformation($"Received Result for Token {scrapeResult.Token}");
                _scraperRepository.AddScrapeResult(scrapeResult.Token, scrapeResult.data);
                _logger.LogInformation("Saved result to Db");
            }
        }

        private async Task DeleteMessageFromQueue(CancellationToken stoppingToken, Message message)
        {
            var deleteResponse = await _resultsQueue.DeleteMessageAsync(message, stoppingToken);

            if (deleteResponse)
                _logger.LogInformation("Message Deleted from Queue");
            else
                _logger.LogInformation("Problem Deleting message from Queue");
        }

        private bool isMessageAvailable(ReceiveMessageResponse response)
        {
            if (response == null)
                return false;

            return response.HttpStatusCode == HttpStatusCode.OK && response.Messages.Any();
        }
    }
}