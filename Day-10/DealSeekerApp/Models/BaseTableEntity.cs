using Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Azure.Data.Tables;

namespace DealSeekerApp.Models
{
    public class BaseTableEntity : ITableEntity
    {
        [JsonIgnore]
        public string PartitionKey { get; set; } = DateTime.Now.ToString("yyyyMMdd");
        [JsonIgnore]
        public string RowKey { get; set; } = Guid.NewGuid().ToString();
        [JsonIgnore]
        public DateTimeOffset? Timestamp { get; set; } = default;
        [JsonIgnore]
        public ETag ETag { get; set; } = default;
    }
}
