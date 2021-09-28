#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["UrlScraper.Api/UrlScraper.Api.csproj", "UrlScraper.Api/"]
COPY ["UrlScraper.Shared/UrlScraper.Shared.csproj", "UrlScraper.Shared/"]
COPY ["UrlScraper.Data/UrlScraper.Data.csproj", "UrlScraper.Data/"]
RUN dotnet restore "UrlScraper.Api/UrlScraper.Api.csproj"
COPY . .
WORKDIR "/src/UrlScraper.Api"
RUN dotnet build "UrlScraper.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UrlScraper.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UrlScraper.Api.dll"]
