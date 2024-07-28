using FindImageApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindImageApp.Services
{
    public interface IUnsplashClient
    {
        Task<List<Image>> GetImagesAsync(string query);
        Task<byte[]> DownloadImageAsync(Uri downloadUrl);
    }
}
