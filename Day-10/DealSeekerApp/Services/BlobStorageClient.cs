using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using DealSeekerApp.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* https://www.c-sharpcorner.com/article/azure-blob-storage-in-c-sharp/ */
namespace DealSeekerApp.Services
{
    public class BlobStorageClient : IBlobStorageClient
    {
        private BlobServiceClient blobServiceClient;
        private readonly ILogger<BlobStorageClient> _logger;
        private readonly BlobStorageClientSettings _settings;

        public BlobStorageClient(ILogger<BlobStorageClient> logger, IOptions<BlobStorageClientSettings> settings)
        {
            _logger = logger;
            _settings = settings.Value;
            blobServiceClient = new BlobServiceClient(_settings.ConnectionString);
        }

        public async Task UploadBlobAsync(string containerName, string blobName, MemoryStream stream, string contentType)
        {
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            await blobClient.UploadAsync(stream, new BlobHttpHeaders() { ContentType = contentType });
        }

        public async Task UploadBlobAsync(string containerName, string blobName, string filePath)
        {
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            using FileStream fs = File.OpenRead(filePath);
            await blobClient.UploadAsync(fs, true);
        }

        public async Task DownloadBlobAsync(string containerName, string blobName, string downloadPath)
        {
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            BlobDownloadInfo blobDownloadInfo = await blobClient.DownloadAsync();

            using FileStream fs = File.OpenWrite(downloadPath);
            await blobDownloadInfo.Content.CopyToAsync(fs);
        }

        public async Task<List<string>> ListBlobsAsync(string containerName)
        {
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobs = new List<string>();

            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
            {
                blobs.Add(blobItem.Name);
            }

            return blobs;
        }

        public async Task DeleteBlobAsync(string containerName, string blobName)
        {
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            await blobClient.DeleteIfExistsAsync();
        }

        public async Task DeleteContainerAsync(string containerName)
        {
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            await containerClient.DeleteIfExistsAsync();
        }

        public async Task<IDictionary<string, string>> GetBlobMetadataAsync(string containerName, string blobName)
        {
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            BlobProperties properties = await blobClient.GetPropertiesAsync();
            return properties.Metadata;
        }
    }

}
