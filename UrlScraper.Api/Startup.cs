using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using UrlScraper.Api.BackgroundServices;
using UrlScraper.Api.Options;
using UrlScraper.Data.Repository;
using UrlScraper.Shared;
using UrlScraper.Shared.Options;
using UrlScraper.Shared.SqsQueue;

namespace UrlScraper.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "URL Scraper API", Version = "v1" });
            });


            services.AddSingleton(_ => new QueueOptions()
            {
                RequestOptions = GetRequestSqsConfigurator(),
                ResultOptions = GetResultSqsConfigurator()
            });

            services.AddScoped<SqsQueueFactory>();
            services.AddSingleton<IUrlScraperRepositoryFactory>(_=> new UrlScraperRepositoryFactory(Configuration.GetSection("ConnectionString").Value));

            services.AddHostedService(p => new UrlScraperResultsProcessor(
                p.GetRequiredService<ILogger<UrlScraperResultsProcessor>>(),
                new SqsQueue(p.GetRequiredService<ILogger<SqsQueue>>(),
                    GetResultSqsConfigurator()),
                p.GetRequiredService<IUrlScraperRepositoryFactory>()));

        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "UrlScraper API v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }


        private SqsConfigurator GetRequestSqsConfigurator()
        {
            return new SqsConfigurator()
            {
                AccessKey = Configuration.GetSection("AwsScraperRequestQueueConfiguration").GetSection("AccessKey").Value,
                QueueUrl = Configuration.GetSection("AwsScraperRequestQueueConfiguration").GetSection("QueueUrl").Value,
                Region = Configuration.GetSection("AwsScraperRequestQueueConfiguration").GetSection("Region").Value,
                SecretKey = Configuration.GetSection("AwsScraperRequestQueueConfiguration").GetSection("SecretKey").Value
            };
        }

        private SqsConfigurator GetResultSqsConfigurator()
        {
            return new SqsConfigurator()
            {
                AccessKey = Configuration.GetSection("AwsScraperResultsQueueConfiguration").GetSection("AccessKey").Value,
                QueueUrl = Configuration.GetSection("AwsScraperResultsQueueConfiguration").GetSection("QueueUrl").Value,
                Region = Configuration.GetSection("AwsScraperResultsQueueConfiguration").GetSection("Region").Value,
                SecretKey = Configuration.GetSection("AwsScraperResultsQueueConfiguration").GetSection("SecretKey").Value
            };
        }
    }
}
