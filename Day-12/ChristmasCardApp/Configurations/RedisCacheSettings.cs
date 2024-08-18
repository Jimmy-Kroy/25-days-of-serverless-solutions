using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChristmasCardApp.Configurations
{
    public class RedisCacheSettings
    {
        public required string InstanceName { get; set; }
        public required string ConnectionString { get; set; }
        //{"timespan":"1.02:03:04"}
        //Timespan format in .net core is D.HH:mm:nn (so "1.02:03:04" is 1 day, 2 hours, 3 mins, 4 seconds).
        public required TimeSpan CachingTime { get; set; }
    }
}
