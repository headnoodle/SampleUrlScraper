using Microsoft.EntityFrameworkCore;
using UrlScraper.Data.Models;

namespace UrlScraper.Data
{
    public class ScraperDbContext : DbContext
    {
        public ScraperDbContext():base() { }

        public ScraperDbContext(string connectionString) : base(GetOptions(connectionString))
        {
        }

        private static DbContextOptions GetOptions(string connectionString)
        {
            return new DbContextOptionsBuilder().UseSqlServer(connectionString).Options;
        }

        public DbSet<ScrapeRequest> ScrapeRequests { get; set; }
        public DbSet<ScrapeResult> ScrapeRequestResults { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer();
        }
    }
}