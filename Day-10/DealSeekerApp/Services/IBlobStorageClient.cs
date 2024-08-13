using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DealSeekerApp.Services
{
    public interface IBlobStorageClient
    {
        Task DeleteBlobAsync(string containerName, string blobName);
        Task DeleteContainerAsync(string containerName);
        Task DownloadBlobAsync(string containerName, string blobName, string downloadPath);
        Task<IDictionary<string, string>> GetBlobMetadataAsync(string containerName, string blobName);
        Task<List<string>> ListBlobsAsync(string containerName);
        Task UploadBlobAsync(string containerName, string blobName, MemoryStream stream, string contentType);
        Task UploadBlobAsync(string containerName, string blobName, string filePath);
    }
}
