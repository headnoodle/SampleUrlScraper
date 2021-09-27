﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UrlScraper.Data;

namespace UrlScraper.Data.Migrations
{
    [DbContext(typeof(ScraperDbContext))]
    partial class ScraperDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.10")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("UrlScraper.Data.Models.ScrapeRequest", b =>
                {
                    b.Property<int>("ScrapeRequestId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Processed")
                        .HasColumnType("bit");

                    b.Property<string>("Url")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ScrapeRequestId");

                    b.ToTable("ScrapeRequests");
                });

            modelBuilder.Entity("UrlScraper.Data.Models.ScrapeResult", b =>
                {
                    b.Property<int>("ScrapeResultId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ResultData")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ScrapeRequestId")
                        .HasColumnType("int");

                    b.HasKey("ScrapeResultId");

                    b.HasIndex("ScrapeRequestId")
                        .IsUnique();

                    b.ToTable("ScrapeRequestResults");
                });

            modelBuilder.Entity("UrlScraper.Data.Models.ScrapeResult", b =>
                {
                    b.HasOne("UrlScraper.Data.Models.ScrapeRequest", null)
                        .WithOne("ScrapeResult")
                        .HasForeignKey("UrlScraper.Data.Models.ScrapeResult", "ScrapeRequestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("UrlScraper.Data.Models.ScrapeRequest", b =>
                {
                    b.Navigation("ScrapeResult");
                });
#pragma warning restore 612, 618
        }
    }
}