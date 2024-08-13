using Azure.Data.Tables;
using Azure;
using DealSeekerApp.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DealSeekerApp.Services
{
    public class TableStorageClient<T> where T : class, ITableEntity, new()
    {
        private readonly TableServiceClient _tableServiceClient;
        private readonly TableClient _tableClient;
        private readonly ILogger<TableStorageClient<T>> _logger;
        private readonly TableStorageClientSettings _settings;

        public TableStorageClient(ILogger<TableStorageClient<T>> logger, IOptions<TableStorageClientSettings> settings)
        {
            _logger = logger;
            _settings = settings.Value;
            _tableServiceClient = new TableServiceClient(_settings.ConnectionString);
            _tableClient = _tableServiceClient.GetTableClient(tableName: _settings.TableName);
        }

        public async Task<int> AddItemAsync(T item)
        {
            Response response = await _tableClient.AddEntityAsync<T>(item);
            return response.Status;
        }

        public async Task BatchInsertAsync(IEnumerable<T> itemList)
        {
            // Create the batch.
            List<TableTransactionAction> addEntitiesBatch = new List<TableTransactionAction>();

            //Add the entities to be added to the batch.
            addEntitiesBatch.AddRange(itemList.Select(e => new TableTransactionAction(TableTransactionActionType.Add, e)));
            Response<IReadOnlyList<Response>> response = await _tableClient.SubmitTransactionAsync(addEntitiesBatch).ConfigureAwait(false);

            _logger.LogInformation($"Status: {response.GetRawResponse().Status}, ReasonPhrase: {response.GetRawResponse().ReasonPhrase}");

            for (int i = 0; i < itemList.Count(); i++)
            {
                _logger.LogInformation($"The ETag for the entity with RowKey: '{itemList.ElementAt(i).RowKey}' is {response.Value[i].Headers.ETag}");
            }
        }

        public async Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate)
        {
            List<T> items = new List<T>();

            AsyncPageable<T> query = _tableClient.QueryAsync<T>(predicate);
            await foreach (Page<T> page in query.AsPages())
            {
                items.AddRange(page.Values);
            }
            return items;
        }
    }
}
