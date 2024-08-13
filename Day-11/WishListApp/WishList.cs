using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WishListApp.Models;
using WishListApp.Services;
//Install this nuget package for the CosmosDBTrigger
using Microsoft.Azure.Functions.Worker.Extensions.CosmosDB;

namespace WishListApp
{
    public class WishList
    {
        private readonly ILogger<WishList> _logger;
        private readonly ICosmosDbService<Wish> _cosmosDbService;
        private readonly ISlackClient<Wish> _slackClient;

        public WishList(ILogger<WishList> logger, ICosmosDbService<Wish> cosmosDbService, ISlackClient<Wish> slackClient)
        {
            _logger = logger;
            _cosmosDbService = cosmosDbService;
            _slackClient = slackClient;
        }

        [Function("AddWish")]
        public async Task<IActionResult> AddWish([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
        {
            _logger.LogInformation("AddWish HTTP trigger function processed a request.");
            IFormCollection form = await req.ReadFormAsync();

            _logger.LogInformation($"Rcvd: addr[{form["addr"]}], desc[{form["desc"]}], who[{form["who"]}], type[{form["type"]}]");

            Wish wish = new Wish()
            {
                Address = form["addr"],
                Description = form["desc"],
                Who = form["who"],
                Type = form["type"]
            };

            _logger.LogInformation($"Created new Wish object with id[{wish.Id}].");
            await _cosmosDbService.AddAsync(wish);

            return new OkObjectResult($"Hello, {wish.Who}. Your wish has been submitted to Santa.");
        }

        [Function("GetAllWishes")]
        public async Task<IActionResult> GetAllWishes([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            string jsonResponse;
            const string query = "SELECT* FROM C";


            _logger.LogInformation("GetAllWishes HTTP trigger function processed a request.");

            IEnumerable<Wish> wishes = await _cosmosDbService.GetMultipleAsync(query);

            if (wishes.Any())
            {
                _logger.LogInformation($"{wishes.Count()} wishes found.");
                jsonResponse = JsonConvert.SerializeObject(wishes, Formatting.Indented);
            }
            else
            {
                _logger.LogInformation("No wishes found.");
                jsonResponse = "No wishes found.";
            }

            return new OkObjectResult(jsonResponse);
        }

        [Function("NewWishNotification")]
        public async Task NewWishNotification([CosmosDBTrigger(
        databaseName: "santa-db",
        containerName: "wishes",
        Connection = "CosmosDBConnectionString",
        LeaseContainerName = "leases",
        CreateLeaseContainerIfNotExists = true)] IReadOnlyList<Wish> wishes)
        {
            _logger.LogInformation($"Wish notification received, count[{wishes.Count()}].");

            List<Task> tasks = new List<Task>(wishes.Count());

            foreach (Wish wish in wishes)
            {
                _logger.LogInformation($"Id[{wish.Id}], who[{wish.Who}] description[{wish.Description}], address[{wish.Address}], type[{wish.Type}]");
                tasks.Add(_slackClient.PostAsync(wish));
                //bool IsSuccess = await _slackClient.PostAsync(wish);
            }

            await Task.WhenAll(tasks);
        }
    }
}
