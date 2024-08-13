using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WishListApp.Configurations
{
    public class CosmosDbSettings
    {
        /// <summary>
        ///     CosmosDb Account - The Azure Cosmos DB endpoint
        /// </summary>
        public required string EndpointUrl { get; set; }
        /// <summary>
        ///     Key - The primary key for the Azure DocumentDB account.
        /// </summary>
        public required string PrimaryKey { get; set; }
        /// <summary>
        ///     Database name
        /// </summary>
        public required string DatabaseName { get; set; }
        /// <summary>
        ///     Container in the database
        /// </summary>
        public required ContainerInfo Container { get; set; }
    }
}
