using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UrlScraper.Processor.BackgroundServices;
using UrlScraper.Processor.ScrapeRequestProcessors;
using UrlScraper.Shared;
using UrlScraper.Shared.Options;
using UrlScraper.Shared.SqsQueue;


namespace UrlScraper.Processor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;

                    services.AddSingleton<IScrapeRequestProcessorFactory, ScrapeRequestProcessorFactory>();
                    services.AddSingleton<IScrapeRequestProcessorFactory, ScrapeRequestProcessorFactory>();

                    services.AddHostedService<UrlScraperService>(CreateScraperService(configuration));
                });

        private static Func<IServiceProvider, UrlScraperService> CreateScraperService(IConfiguration configuration)
        {
            return U => new UrlScraperService(
                U.GetRequiredService<ILogger<UrlScraperService>>(),
                new SqsQueue(U.GetRequiredService<ILogger<SqsQueue>>(),
                    new SqsConfigurator()
                    {
                        AccessKey = configuration.GetSection("AwsScraperRequestQueueConfiguration").GetSection("AccessKey").Value,
                        QueueUrl = configuration.GetSection("AwsScraperRequestQueueConfiguration").GetSection("QueueUrl").Value,
                        Region = configuration.GetSection("AwsScraperRequestQueueConfiguration").GetSection("Region").Value,
                        SecretKey = configuration.GetSection("AwsScraperRequestQueueConfiguration").GetSection("SecretKey").Value
                    }),
                new SqsQueue(U.GetRequiredService<ILogger<SqsQueue>>(),
                    new SqsConfigurator()
                    {
                        AccessKey = configuration.GetSection("AwsScraperResultsQueueConfiguration").GetSection("AccessKey").Value,
                        QueueUrl = configuration.GetSection("AwsScraperResultsQueueConfiguration").GetSection("QueueUrl").Value,
                        Region = configuration.GetSection("AwsScraperResultsQueueConfiguration").GetSection("Region").Value,
                        SecretKey = configuration.GetSection("AwsScraperResultsQueueConfiguration").GetSection("SecretKey").Value
                    }),
                U.GetRequiredService<IScrapeRequestProcessorFactory>());
        }
    }
}
