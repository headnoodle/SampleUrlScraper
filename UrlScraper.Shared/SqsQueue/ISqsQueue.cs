using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS.Model;

namespace UrlScraper.Shared.SqsQueue
{
    public interface ISqsQueue
    {
        Task<bool> DeleteMessageAsync(Message message,  CancellationToken stoppingToken);
        Task<ReceiveMessageResponse> ReceiveMessageResponse(int maxNumberOfMessages, int waitTimeInSeconds, CancellationToken stoppingToken);
        Task<bool> SendMessageAsync(string message, CancellationToken cancellationToken);
    }
}