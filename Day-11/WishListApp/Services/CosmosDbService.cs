using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WishListApp.Configurations;

namespace WishListApp.Services
{
    public class CosmosDbService<T> : ICosmosDbService<T>
    {
        private readonly CosmosDbSettings _cosmosDbSettings;
        private readonly CosmosClient _cosmosClient;
        private readonly Container _container;
        ILogger<CosmosDbService<T>> _logger;

        public CosmosDbService(IOptions<CosmosDbSettings> cosmosDbSettings, ILogger<CosmosDbService<T>> logger)
        {
            _logger = logger;
            _cosmosDbSettings = cosmosDbSettings.Value;
            _cosmosClient = new CosmosClient(_cosmosDbSettings.EndpointUrl, _cosmosDbSettings.PrimaryKey,
                new CosmosClientOptions() { AllowBulkExecution = true });
            _container = _cosmosClient.GetContainer(_cosmosDbSettings.DatabaseName, _cosmosDbSettings.Container.Name);

        }

        public async Task AddAsync(T item)
        {
            ItemResponse<T> response = await _container.CreateItemAsync(item);
            _logger.LogInformation($"AddAsync {response.RequestCharge} RUs for this call.");
        }

        public async Task BulkInsertAsync(IEnumerable<T> items)
        {
            List<Task> tasks = new List<Task>(items.Count());

            foreach (T item in items)
            {
                tasks.Add(_container.CreateItemAsync(item)
                    .ContinueWith(itemResponse =>
                    {
                        if (itemResponse is not null && itemResponse.IsCompletedSuccessfully)
                        {
                            _logger.LogInformation($"BulkInsertAsync {itemResponse.Result.RequestCharge} RUs for this call.");
                        }
                        else if (itemResponse is not null && itemResponse.Exception is not null)
                        {
                            AggregateException innerExceptions = itemResponse.Exception.Flatten();
                            if (innerExceptions.InnerExceptions.FirstOrDefault(innerEx => innerEx is CosmosException) is CosmosException cosmosException)
                            {
                                _logger.LogError($"Received {cosmosException.StatusCode} ({cosmosException.Message}).");
                            }
                            else
                            {
                                _logger.LogError($"Exception {innerExceptions.InnerExceptions.FirstOrDefault()}.");
                            }
                        }
                    }));
            }

            // Wait until all are done
            await Task.WhenAll(tasks);
        }

        public async Task DeleteAsync(string id)
        {
            ItemResponse<T> response = await _container.DeleteItemAsync<T>(id, new PartitionKey(id));
            _logger.LogInformation($"DeleteAsync {response.RequestCharge} RUs for this call.");
        }

        public async Task<T> GetAsync(string id)
        {
            try
            {
                ItemResponse<T> response = await _container.ReadItemAsync<T>(id, new PartitionKey(id));
                _logger.LogInformation($"GetAsync {response.RequestCharge} RUs for this call.");

                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return default(T)!;
            }
            catch (CosmosException ex)
            {
                _logger.LogInformation($"CosmosException in GetAsync(string id): {ex.ToString()}");
                return default(T)!;
            }
        }

        public async Task<IEnumerable<T>> GetMultipleAsync(string queryString)
        {
            FeedIterator<T> feedIterator = _container.GetItemQueryIterator<T>(new QueryDefinition(queryString));
            var results = new List<T>();
            while (feedIterator.HasMoreResults)
            {
                FeedResponse<T> response = await feedIterator.ReadNextAsync();
                _logger.LogInformation($"GetMultipleAsync {response.RequestCharge} RUs for this call.");
                results.AddRange(response.ToList());
            }
            return results;
        }

        public async Task UpdateAsync(T item)
        {
            ItemResponse<T> response = await _container.UpsertItemAsync(item);
            _logger.LogInformation($"UpdateAsync {response.RequestCharge} RUs for this call.");
            //ItemResponse<T> response = await _container.ReplaceItemAsync(item, id, new PartitionKey(id));
            //_logger.LogInformation($"UpdateAsync {response.RequestCharge} RUs for this call.");
        }
    }

}
