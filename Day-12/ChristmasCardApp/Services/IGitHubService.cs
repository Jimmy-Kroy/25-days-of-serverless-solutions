using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChristmasCardApp.Services
{
    public interface IGitHubService
    {
        Task<string?> GetContentAsync(string name, bool convertToHtml = false);
    }
}
