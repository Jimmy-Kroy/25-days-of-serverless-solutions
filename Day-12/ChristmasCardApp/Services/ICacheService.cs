using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChristmasCardApp.Services
{
    public interface ICacheService
    {
        Task SetStringAsync(string key, string value);
        Task<string?> GetStringAsync(string key);
    }
}
