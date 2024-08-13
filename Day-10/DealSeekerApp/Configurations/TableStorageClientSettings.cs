using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DealSeekerApp.Configurations
{
    public class TableStorageClientSettings
    {
        public required string ConnectionString { get; set; }
        public required string TableName { get; set; }
    }
}
