using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using PotluckManagerApp.Models;
using PotluckManagerApp.Services;
using Newtonsoft.Json;
using PotluckManagerApp.Helpers;
//using System.Text.Json;

namespace PotluckManagerApp
{
    public class PotluckManager
    {
        private readonly ILogger<PotluckManager> _logger;
        private readonly ICosmosDbService<FoodDish> _cosmosDbService;

        public PotluckManager(ILogger<PotluckManager> logger, ICosmosDbService<FoodDish> cosmosDbService)
        {
            _logger = logger;
            _cosmosDbService = cosmosDbService;
        }

        [Function("GetAllFoodDishes")]
        public IActionResult GetAllFoodDishes([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "PotluckManager/GetAllFoodDishes")] HttpRequest req)
        {
            string jsonString;

            _logger.LogInformation("GetAllFoodDishes   C# HTTP trigger function processed a request.");

            List<FoodDish> foodDishes = new List<FoodDish>();

            //foodDishes.Add(new FoodDish("Kyle Reese", "Macaroni And Cheese", "For about six hungry persons!", true));
            //foodDishes.Add(new FoodDish("Jennifer van Dijk", "Slow-Cooker Grape Jelly Meatballs.", "Twenty balls of 100 gram each.", false));
            //foodDishes.Add(new FoodDish("Chantal Janszen", "Broccoli, Grape, And Pasta Salad.", "Four person salad.", true));
            //foodDishes.Add(new FoodDish("Mohammed Dunya", "Classic Potato Salad.", "Five people.", true));

            if (foodDishes.Any())
            {
                _logger.LogInformation($"{foodDishes.Count()} food dishes found.");
                jsonString = System.Text.Json.JsonSerializer.Serialize(foodDishes);
            }
            else
            {
                _logger.LogInformation("No food dishes found.");
                jsonString = "No food dishes found.";
            }

            return new OkObjectResult(jsonString);
        }

        [Function("GetFoodDishById")]
        public IActionResult GetFoodDishById([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "PotluckManager/GetFoodDishById/{id}")] HttpRequest req,
            string id)
        {
            _logger.LogInformation($"GetFoodDishById  id[{id}]  C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions! GetFoodDishById");
        }

        [Function("AddFoodDishes")]
        public async Task<IActionResult> AddFoodDishes([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "PotluckManager/AddFoodDishes")] HttpRequest req)
        {
            _logger.LogInformation("AddFoodDishes HTTP post trigger function received a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            if (string.IsNullOrEmpty(requestBody))
            {
                return new OkObjectResult("No JSON file received!");
            }
            else if (!requestBody.IsValidJson(out string errorMessage))
            {
                return new BadRequestObjectResult("Please check and validate the JSON file! " + errorMessage);
            }

            IEnumerable<FoodDish> foodDishes = JsonConvert.DeserializeObject<IEnumerable<FoodDish>>(requestBody)!;

            int cnt = 0;
            foreach(FoodDish foodDish in foodDishes)
            {
                _logger.LogInformation($"FoodDish[{cnt}]: Id: {foodDish.Id}");
                _logger.LogInformation($"FoodDish[{cnt}]: CreationTime: {foodDish.CreationTime}");
                _logger.LogInformation($"FoodDish[{cnt}]: GuestName: {foodDish.GuestName}");
                _logger.LogInformation($"FoodDish[{cnt}]: Dish: {foodDish.Dish}");
                _logger.LogInformation($"FoodDish[{cnt}]: Amount: {foodDish.Amount}");
                _logger.LogInformation($"FoodDish[{cnt}]: IsVegan: {foodDish.IsVegan}");
                cnt++;
            }

            string jsonString = System.Text.Json.JsonSerializer.Serialize(foodDishes);

            _logger.LogInformation($"Saving FoodDish to database ...");

            await _cosmosDbService.BulkInsertAsync(foodDishes);

            return new OkObjectResult(jsonString);
        }

        [Function("DeleteFoodDishById")]
        public IActionResult DeleteFoodDishById([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "PotluckManager/DeleteFoodDishById/{id}")] HttpRequest req,
            string id)
        {
            _logger.LogInformation($"DeleteFoodDish id[{id}]         C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions! DeleteFoodDishById");
        }

        [Function("UpdateFoodDish")]
        public IActionResult UpdateFoodDish([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "PotluckManager/UpdateFoodDish")] HttpRequest req)
        {
            _logger.LogInformation($"UpdateFoodDish         C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions! UpdateFoodDish");
        }
    }
}
