using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using UrlScraper.Api.Models;
using UrlScraper.Api.Options;
using UrlScraper.Data.Repository;
using UrlScraper.Shared;
using UrlScraper.Shared.Models;
using UrlScraper.Shared.SqsQueue;

namespace UrlScraper.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UrlScraperController : ControllerBase
    {

        private readonly ILogger<UrlScraperController> _logger;
        private readonly QueueOptions _queueOptions;
        private readonly SqsQueueFactory _queueFactory;
        private readonly IUrlScraperRepository _urlScraperRepository;

        public UrlScraperController(ILogger<UrlScraperController> logger, QueueOptions queueOptions, SqsQueueFactory queueFactory, IUrlScraperRepositoryFactory urlScraperFactory)
        {
            _logger = logger??throw new ArgumentNullException(nameof(logger));
            _queueOptions = queueOptions??throw new ArgumentNullException(nameof(queueOptions));
            _queueFactory = queueFactory??throw new ArgumentNullException(nameof(queueFactory));
            _urlScraperRepository = urlScraperFactory.Create()??throw new ArgumentNullException(nameof(urlScraperFactory));
        }

        [HttpPost("RequestUrlScrape")]
        public async Task<StandardResponse<RequestUrlScrapeResponse>> RequestUrlScrape(string urlToScrape, CancellationToken cancellationToken)
        {

            try
            {
                if (IsUrlValid(urlToScrape))
                {
                    Guid requestToken = SaveRequestToStore(urlToScrape);

                    if (await SendToQueue(urlToScrape, requestToken, cancellationToken))
                        return new StandardResponse<RequestUrlScrapeResponse>() { Successful = true, Payload = new RequestUrlScrapeResponse() {Token = requestToken } };
                    else
                    {
                        return StandardResponseError<RequestUrlScrapeResponse>("Problem adding to processing queue", 2);
                    }
                }

                return StandardResponseError<RequestUrlScrapeResponse>("Url invalid", 1);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Problem Adding new URL scrape");
                return StandardResponseError<RequestUrlScrapeResponse>("Fatal Error", -1);
            }

        }

        [HttpGet("GetRegisteredScrapes")]
        public StandardResponse<IEnumerable<RegisteredScrapeStatusResponse>> ListRegisteredOperations()
        {
            try
            {
                var requests = _urlScraperRepository.GetAllRequests()
                    .Select(r => new RegisteredScrapeStatusResponse()
                    {
                        Token = r.Token,
                        Url = r.Url,
                        Processed = r.Processed
                    });

                return new StandardResponse<IEnumerable<RegisteredScrapeStatusResponse>>() { Payload = requests, Successful = true };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Problem listing registered requests");
                return StandardResponseError<IEnumerable<RegisteredScrapeStatusResponse>>("Fatal Error", -1);

            }
        }

        [HttpGet("GetCompletedScrape")]
        public StandardResponse<UrlScrapeResultResponse> GetCompletedScrapeResult(Guid token)
        {
            try
            {
                var successfulScrape = _urlScraperRepository.GetScraperResultForToken(token);

                if (successfulScrape != null)
                    return new StandardResponse<UrlScrapeResultResponse>(){ Successful  = true, Payload = new UrlScrapeResultResponse(){ScrapeResults = successfulScrape.ResultData, Token = successfulScrape.ScrapeRequestId}};

                return StandardResponseError<UrlScrapeResultResponse>("Scrape result not found", 4);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Problem getting completed scrape result");
                return StandardResponseError<UrlScrapeResultResponse>("Fatal Error", -1);
            }

        }

        private async Task<bool> SendToQueue(string url, Guid requestToken, CancellationToken cancellationToken)
        {
            var urlScrapeRequest = new QueueUrlScrapeRequest() { Url = url, Token = requestToken};
            var urlScrapeRequestJson = JsonSerializer.Serialize(urlScrapeRequest);

            var requestQueue = _queueFactory.CreateQueue(_queueOptions.RequestOptions);
            return  await requestQueue.SendMessageAsync(urlScrapeRequestJson, cancellationToken);
        }

        private Guid SaveRequestToStore(string urlToScrape)
        {
            return _urlScraperRepository.AddNewScraperRequest(urlToScrape);
        }

        private static bool IsUrlValid(string url)
        {
            if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
                if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
                    if (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
                        return true;

            return false;
        }

        private static StandardResponse<T> StandardResponseError<T>(string message, int errorNumber)
        {
            return new StandardResponse<T>() { Error = new ErrorResponse() { ErrorMessage = message, ErrorNumber = errorNumber }, Successful = false };
        }


    }

}
