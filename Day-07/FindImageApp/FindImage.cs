using FindImageApp.Models;
using FindImageApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FindImageApp
{
    public class FindImage
    {
        private readonly ILogger<FindImage> _logger;
        private readonly IUnsplashClient _unsplashClient;

        public FindImage(ILogger<FindImage> logger, IUnsplashClient unsplashClient)
        {
            _logger = logger;
            _unsplashClient = unsplashClient;
        }

        [Function("FindImage")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            try
            {
                byte[] fileContent = default;
                bool IsSuccess = false;
                SearchQuery searchQuery = default;
                List<Image> images = default;

                _logger.LogInformation("FindImage HTTP trigger function processed a request.");
                /* Get query. */
                string query = req.Query["query"].ToString();
                IsSuccess = !string.IsNullOrEmpty(query);

                if (IsSuccess)
                {
                    _logger.LogInformation($"Value query get parameter[{query}].");

                    /* Create search object. */
                    searchQuery = new SearchQuery()
                    {
                        Query = query,
                        ReplyMode = SearchQuery.eReplyMode.Url //default
                    };

                    /*  Get return mode, json file or the first image. */
                    string mode = req.Query["mode"].ToString();
                    if (!string.IsNullOrEmpty(mode) && mode == "file")
                    {
                        searchQuery.ReplyMode = SearchQuery.eReplyMode.FileContent;
                    }
                    else if (!string.IsNullOrEmpty(mode) && mode == "redirect")
                    {
                        searchQuery.ReplyMode = SearchQuery.eReplyMode.Redirect;
                    }

                    images = await _unsplashClient.GetImagesAsync(searchQuery.Query);
                    IsSuccess = (images != null) && (images.Count > 0);
                }

                if (IsSuccess && searchQuery.ReplyMode == SearchQuery.eReplyMode.FileContent)
                {
                    Image image = images.FirstOrDefault();
                    fileContent = await _unsplashClient.DownloadImageAsync(image.RegularUrl);
                    FileContentResult fileContentResult = new FileContentResult(fileContent, new Microsoft.Net.Http.Headers.MediaTypeHeaderValue("image/png"))
                    {
                        FileDownloadName = image.FileName
                        //FileDownloadName = (searchQuery.Query + ".png").Replace(' ', '_')
                    };
                    return fileContentResult;
                }
                else if (IsSuccess && searchQuery.ReplyMode == SearchQuery.eReplyMode.Url)
                {
                    string jsonResponse = JsonConvert.SerializeObject(images, Formatting.Indented);
                    return new OkObjectResult(jsonResponse);
                }
                else if (IsSuccess && searchQuery.ReplyMode == SearchQuery.eReplyMode.Redirect)
                {
                    return new RedirectResult(images.First().RegularUrl.ToString(), false);
                }
                else if (!IsSuccess && string.IsNullOrEmpty(query))
                {
                    return new BadRequestObjectResult("Please pass a query parameter in the url.");
                }
                else if (!IsSuccess && (images == null || !images.Any()))
                {
                    return new NotFoundResult();
                }
                else
                {
                    return new BadRequestObjectResult("Something went bad!");
                }
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, e.Message);
                // _logger.LogError(e, e.Message);
                return new StatusCodeResult(500);
            }
        }
    }

}
