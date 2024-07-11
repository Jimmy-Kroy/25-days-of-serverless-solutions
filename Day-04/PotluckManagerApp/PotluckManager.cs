using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using PotluckManagerApp.Models;
using PotluckManagerApp.Services;
using Newtonsoft.Json;
using PotluckManagerApp.Helpers;

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
        public async Task<IActionResult> GetAllFoodDishes([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "PotluckManager/GetAllFoodDishes")] HttpRequest req)
        {
            string jsonString;
            const string query = "SELECT* FROM C";

            _logger.LogInformation("GetAllFoodDishes HTTP get trigger function received a request.");

            IEnumerable<FoodDish> foodDishes = await _cosmosDbService.GetMultipleAsync(query);

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
        public async Task<IActionResult> GetFoodDishById([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "PotluckManager/GetFoodDishById/{id}")] HttpRequest req,
            string id)
        {
            string jsonString;

            _logger.LogInformation("GetFoodDishById HTTP get trigger function received a request.");
            FoodDish foodDish = await _cosmosDbService.GetAsync(id);

            if(foodDish == default(FoodDish))
            {
                return new NotFoundResult();
            }
            else
            {
                jsonString = System.Text.Json.JsonSerializer.Serialize(foodDish);
                return new OkObjectResult(jsonString);
            } 
        }

        [Function("AddFoodDishes")]
        public async Task<IActionResult> AddFoodDishes([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "PotluckManager/AddFoodDishes")] HttpRequest req)
        {
            _logger.LogInformation("AddFoodDishes HTTP post trigger function received a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            if (string.IsNullOrEmpty(requestBody))
            {
                return new BadRequestObjectResult("No JSON file received!");
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
        public async Task<IActionResult> DeleteFoodDishById([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "PotluckManager/DeleteFoodDishById/{id}")] HttpRequest req,
            string id)
        {
            string jsonString;

            _logger.LogInformation("DeleteFoodDishById HTTP delete trigger function received a request.");
            //Get the record so we can return it to the caller.
            FoodDish foodDish = await _cosmosDbService.GetAsync(id);

            if (foodDish == default(FoodDish))
            {
                _logger.LogInformation("DeleteFoodDishById no food dish found corresponding to the provided id.");
                return new NotFoundResult();
            }
            else
            {
                _logger.LogInformation($"DeleteFoodDishById deleting food dish with id[{id}].");
                await _cosmosDbService.DeleteAsync(id);
                jsonString = System.Text.Json.JsonSerializer.Serialize(foodDish);
                return new OkObjectResult(jsonString);
            }
        }

        [Function("UpdateFoodDish")]
        public async Task<IActionResult> UpdateFoodDish([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "PotluckManager/UpdateFoodDish")] HttpRequest req)
        {
            _logger.LogInformation("UpdateFoodDish HTTP put trigger function received a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            if (string.IsNullOrEmpty(requestBody))
            {
                return new BadRequestObjectResult("No JSON file received!");
            }
            else if (!requestBody.IsValidJson(out string errorMessage))
            {
                return new BadRequestObjectResult("Please check and validate the JSON file! " + errorMessage);
            }

            FoodDish foodDishUpdate = JsonConvert.DeserializeObject<FoodDish>(requestBody)!;

            if(string.IsNullOrEmpty(foodDishUpdate.Id.ToString()))
            {
                return new BadRequestObjectResult("Please provide the required food dish id in the JSON file!");
            }
            else if(string.IsNullOrEmpty(foodDishUpdate.Dish) &&
                string.IsNullOrEmpty(foodDishUpdate.GuestName) &&
                string.IsNullOrEmpty(foodDishUpdate.Amount) &&
                foodDishUpdate.IsVegan == null)
            {
                return new BadRequestObjectResult("Please provide fields to update in the JSON file!");
            }

            _logger.LogInformation($"foodDishUpdate: Id: {foodDishUpdate.Id}");
            //CreationTime will not be updated
            //_logger.LogInformation($"foodDishUpdate: CreationTime: {foodDishUpdate.CreationTime}");  
            _logger.LogInformation($"foodDishUpdate: GuestName: {foodDishUpdate.GuestName}");
            _logger.LogInformation($"foodDishUpdate: Dish: {foodDishUpdate.Dish}");
            _logger.LogInformation($"foodDishUpdate: Amount: {foodDishUpdate.Amount}");
            _logger.LogInformation($"foodDishUpdate: IsVegan: {foodDishUpdate.IsVegan}");

            //Retrieve the food dish record
            FoodDish foodDish = await _cosmosDbService.GetAsync(foodDishUpdate.Id.ToString());

            if (foodDish == default(FoodDish))
            {
                _logger.LogInformation("UpdateFoodDish no food dish found corresponding to the provided id.");
                return new NotFoundResult();
            }
            else
            {
                _logger.LogInformation($"foodDish record retrieved from database that will be updated:");
                _logger.LogInformation($"foodDish: Id: {foodDish.Id}");
                _logger.LogInformation($"foodDish: CreationTime: {foodDish.CreationTime}");  
                _logger.LogInformation($"foodDish: GuestName: {foodDish.GuestName}");
                _logger.LogInformation($"foodDish: Dish: {foodDish.Dish}");
                _logger.LogInformation($"foodDish: Amount: {foodDish.Amount}");
                _logger.LogInformation($"foodDish: IsVegan: {foodDish.IsVegan}");

                if (!string.IsNullOrEmpty(foodDishUpdate.GuestName))
                {
                    foodDish.GuestName = foodDishUpdate.GuestName;
                }

                if (!string.IsNullOrEmpty(foodDishUpdate.Dish))
                {
                    foodDish.Dish = foodDishUpdate.Dish;
                }

                if (!string.IsNullOrEmpty(foodDishUpdate.Amount))
                {
                    foodDish.Amount = foodDishUpdate.Amount;
                }

                if (foodDishUpdate.IsVegan != null)
                {
                    foodDish.IsVegan = foodDishUpdate.IsVegan;
                }

                await _cosmosDbService.UpdateAsync(foodDish);
            }

            return new OkObjectResult("Successfully processed the update request.");
        }
    }
}
