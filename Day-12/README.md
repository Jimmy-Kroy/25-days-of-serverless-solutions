# Solution
I created the ChristmasCardApp using an Azure HTTP triggered function. The function is called by passing a query parameters in the GET request, as shown below.

## The end point: [GET] /api/ChristmasCard
```
[GET] /api/ChristmasCard?name=Bart-Simpson
[GET] /api/ChristmasCard?name=Lisa-Simpson
```
Each name passed to the ChristmasCard service must have a corresponding markdown file <a href="https://github.com/Jimmy-Kroy/25-days-of-serverless-solutions/tree/master/Day-12/ChristmasCards" target="_blank">here</a>.

The local.settings.json looks like this:
```json
{
    "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "RedisCache:InstanceName": "ChristmasCardApp",
    "RedisCache:ConnectionString": "<<Redis ConnectionString>>",
    "RedisCache:CachingTime": "1.12:00:00",
    "Github:EndpointUrl": "https://api.github.com/repos/Jimmy-Kroy/25-days-of-serverless-solutions/contents/Day-12/ChristmasCards/"
  }
}
``` 

## Resources/Tools Used

-   **[REST API endpoints for repository contents](https://docs.github.com/en/rest/repos/contents)**
-   **[Use Azure Cache for Redis](https://learn.microsoft.com/en-us/azure/azure-cache-for-redis/cache-dotnet-how-to-use-azure-redis-cache)**
-   **[Implementing caching using DI](https://medium.com/@brucycenteio/implementing-caching-in-asp-net-core-net-7-or-8-enhancing-application-performance-0de59dd6cf67)**


# Challenge 12: Caching

![Challenge 12: Caching](https://res.cloudinary.com/jen-looper/image/upload/v1575988577/images/challenge-12_zfltja.jpg)

## The Challenge

Today in London, Simona wants to send beautiful holiday cards to all her friends But since she's worried they won't arrive in time, she doesn't want to hand-write and mail each individual letter!

Instead, she wants to make each of her friends a personalized website containing a holiday letter for them! She plans on writing each letter as its own Markdown file, and needs a way to turn those into websites.

Create a service that reads Markdown text from GitHub (perhaps using the Gist API), parses the Markdown to HTML, and returns the HTML to the client.

As a bonus challenge: reading and parsing Markdown is a lot of work! To optimize, cache your responses and send cached versions of the processed Markdown.