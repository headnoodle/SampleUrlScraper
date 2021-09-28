# SampleUrlScraper

Instructions to Set up
----------------------
1) replace AWS SQS details with the correct values in \UrlScraper.Api\appsettings.json
2) replace ConnectionString with the correct values in \UrlScraper.Api\appsettings.json
3) replace AWS SQS details with the correct values in \UrlScraper.Processor\appsettings.json

Docker Compose support
----------------------
To run locally

docker-compose up
Navigate to http://localhost:8080/swagger/index.html


Horizontal Scaling
------------------
As this is a queue based system the UrlScraper processor can be scaled by running more instances.
UrlScraper API can be scaled by putting behind a load balancer and firing up more instances of the API.

Other Notes
-----------

The queues are configured support a dead letter queue to put messages that have been taken but have not been deleted after multiple attempts.