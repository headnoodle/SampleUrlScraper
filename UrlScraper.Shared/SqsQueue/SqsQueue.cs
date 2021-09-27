using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Logging;
using UrlScraper.Shared.Options;

namespace UrlScraper.Shared.SqsQueue
{
    public class SqsQueue : ISqsQueue
    {
        private readonly ILogger<SqsQueue> _logger;
        private readonly SqsConfigurator _configOptions;
        private AmazonSQSClient _amazonClient;

        public SqsQueue(ILogger<SqsQueue> logger, SqsConfigurator configOptions)
        {
            _logger = logger?? throw new NullReferenceException(nameof(logger));
            _configOptions = configOptions?? throw new NullReferenceException(nameof(configOptions));
        }
        
        public async Task<bool> DeleteMessageAsync(Message message,  CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Deleting message with ID: {message.MessageId}");
            var deleteResponse = await GetClient().DeleteMessageAsync(_configOptions.QueueUrl, message.ReceiptHandle, stoppingToken);

            if (deleteResponse.HttpStatusCode == HttpStatusCode.OK)
                _logger.LogInformation($"Deleted message with ID: {message.MessageId}");
            else
                _logger.LogInformation($"Failed to deleted message with ID: {message.MessageId}");

            return (deleteResponse.HttpStatusCode == HttpStatusCode.OK);
        }

        public async Task<ReceiveMessageResponse> ReceiveMessageResponse(int maxNumberOfMessages, int waitTimeInSeconds, CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Looking for {maxNumberOfMessages} messages over {waitTimeInSeconds} seconds");
            var response = await GetClient().ReceiveMessageAsync(CreateReceiveMessageRequest(maxNumberOfMessages, waitTimeInSeconds), stoppingToken);

            if (response.HttpStatusCode == HttpStatusCode.OK && response.Messages.Any())
                _logger.LogInformation($"Received {response.Messages.Count()} messages");
            else
                _logger.LogInformation($"Received No Messages");

            return response;
        }

        public async Task<bool> SendMessageAsync(string message, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Adding a new Message to the Queue: {message}");

            var response = await GetClient().SendMessageAsync(_configOptions.QueueUrl, message, cancellationToken);

            if (response.HttpStatusCode==HttpStatusCode.OK)
                _logger.LogInformation("Added a new message to the Queue");
            else
                _logger.LogInformation("Failed to add a new message to the Queue");

            return response.HttpStatusCode == HttpStatusCode.OK;
        }

        private  AmazonSQSClient GetClient()
        {
            if (_amazonClient == null) 
                CreateNewClientInstance();

            return _amazonClient;
        }

        private void CreateNewClientInstance()
        {
            var region = RegionEndpoint.GetBySystemName(_configOptions.Region);
            var credentials = new BasicAWSCredentials(_configOptions.AccessKey, _configOptions.SecretKey);
            _amazonClient = new AmazonSQSClient(credentials, region);
        }

        private ReceiveMessageRequest CreateReceiveMessageRequest(int maxNumberOfMessages, int waitTimeInSeconds)
        {
            return new ReceiveMessageRequest()
            {
                QueueUrl = _configOptions.QueueUrl,
                MaxNumberOfMessages = maxNumberOfMessages,
                WaitTimeSeconds = waitTimeInSeconds
            };
        }

    }
}