#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["UrlScraper.Processor/UrlScraper.Processor.csproj", "UrlScraper.Processor/"]
COPY ["UrlScraper.Shared/UrlScraper.Shared.csproj", "UrlScraper.Shared/"]
RUN dotnet restore "UrlScraper.Processor/UrlScraper.Processor.csproj"
COPY . .
WORKDIR "/src/UrlScraper.Processor"
RUN dotnet build "UrlScraper.Processor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UrlScraper.Processor.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UrlScraper.Processor.dll"]
