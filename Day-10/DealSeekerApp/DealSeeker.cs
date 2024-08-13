using System;
using DealSeekerApp.Models;
using DealSeekerApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DealSeekerApp
{
    public class DealSeeker
    {
        private readonly ILogger _logger;
        private readonly IDealsSeeker _dealsSeeker;
        private readonly IHtmlGeneratorService _htmlGeneratorService;
        private readonly IBlobStorageClient _blobStorageClient;
        private readonly TableStorageClient<DealTableEntity> _tableStorageClient;

        public DealSeeker(ILoggerFactory loggerFactory,
            IDealsSeeker dealsSeeker,
            IHtmlGeneratorService htmlGeneratorService,
            IBlobStorageClient blobStorageClient,
            TableStorageClient<DealTableEntity> tableStorageClient)
        {
            _logger = loggerFactory.CreateLogger<DealSeeker>();
            _dealsSeeker = dealsSeeker;
            _htmlGeneratorService = htmlGeneratorService;
            _blobStorageClient = blobStorageClient;
            _tableStorageClient = tableStorageClient;
        }

        //0 */5 * * * *	once every five minutes
        //0 0 * * * *	once at the top of every hour
        [Function("DealSeeker")]
        public async Task Run([TimerTrigger("0 0 * * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation($"DealSeeker Timer trigger function executed at: {DateTime.Now}");

            //Find deals of the day.
            IEnumerable<DealTableEntity> deals = await _dealsSeeker.GetDealsOfTheDay();

            //Store the deals in the database. 
            await _tableStorageClient.BatchInsertAsync(deals);

            //Create HTML page
            string htmlPage = _htmlGeneratorService.GenerateHtmlPage(deals);

            //Convert String to Stream
            byte[] _byteArray = Encoding.UTF8.GetBytes(htmlPage);
            MemoryStream stream = new MemoryStream(_byteArray);

            //Create static website.
            await _blobStorageClient.UploadBlobAsync("$web", "index.html", stream, "text/html");

            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }
        }

        [Function("GetDealsOfTheDay")]
        public async Task<IActionResult> GetDealsOfTheDay([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            bool IsSuccess = false;
            string jsonResponse = string.Empty;

            _logger.LogInformation("GetDealsOfTheDay HTTP get trigger function received a request.");

            string today = DateTime.Now.ToString("yyyyMMdd");
            IEnumerable<DealTableEntity> deals = await _tableStorageClient.GetItemsAsync(x => x.PartitionKey == today);

            IsSuccess = deals != null && deals.Any();

            if (IsSuccess)
            {
                jsonResponse = JsonConvert.SerializeObject(deals, Formatting.Indented);
                IsSuccess = !string.IsNullOrEmpty(jsonResponse);
            }

            if (IsSuccess)
            {
                return new OkObjectResult(jsonResponse);
            }
            else
            {
                return new NotFoundResult();
            }
        }
    }
}
