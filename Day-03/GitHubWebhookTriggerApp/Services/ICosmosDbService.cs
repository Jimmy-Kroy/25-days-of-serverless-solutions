using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubWebhookTriggerApp.Services
{
    public interface ICosmosDbService<T>
    {
        Task AddAsync(T item);
        Task BulkInsertAsync(IEnumerable<T> items);
        Task DeleteAsync(string id);
        Task<T> GetAsync(string id);
        Task<IEnumerable<T>> GetMultipleAsync(string queryString);
        Task UpdateAsync(string id, T item);
    }
}
