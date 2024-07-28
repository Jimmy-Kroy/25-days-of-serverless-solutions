using FindImageApp.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FindImageApp.Services
{
    public class UnsplashClient : IUnsplashClient
    {
        private readonly Uri _endpointUrl;
        private readonly string _accessKey;
        private readonly ILogger<UnsplashClient> _logger;
        private readonly HttpClient _httpClient;
        /* https://unsplash.com/documentation#search-photos
         Page number to retrieve. (Optional; default: 1) 
         order_by How to sort the photos. (Optional; default: relevant). */
        const string route = "/search/photos?per_page=3&query=";
        public UnsplashClient(Uri endpointUrl, string accessKey, ILogger<UnsplashClient> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _endpointUrl = endpointUrl;
            _accessKey = accessKey;
            _logger.LogInformation($"UnsplashClient, endpointUrl[{_endpointUrl}], accessKey[{_accessKey}]");
            _httpClient = httpClientFactory.CreateClient();
            /* Set UNSPLASH_ACCESS_KEY in Authorization header. */
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Client-ID {accessKey}");
        }

        public async Task<List<Image>> GetImagesAsync(string query)
        {
            ImageSearchResults? imageSearchResults = null;
            List<Image> images = new List<Image>();
            string jsonResponse = string.Empty;
            bool IsSuccess = false;

            using (HttpRequestMessage request = new HttpRequestMessage())
            {
                // Build the request.
                request.Method = HttpMethod.Get;
                /* Spaces are not allowed in URLs. They should be replaced by the string %20. 
                 * In the query string part of the URL, %20 can be abbreviated using a plus sign (+). */
                string urlEncodedString = HttpUtility.UrlEncode(query);
                request.RequestUri = new Uri(_endpointUrl + route + urlEncodedString);
                _logger.LogInformation($"UnsplashClient, RequestUri(Canonical version)[{request.RequestUri.ToString()}]");
                _logger.LogInformation($"UnsplashClient, RequestUri(OriginalString)[{request.RequestUri.OriginalString}]");

                // Send the request and get response.
                HttpResponseMessage response = await _httpClient.SendAsync(request).ConfigureAwait(false);
                IsSuccess = response.IsSuccessStatusCode;

                if (IsSuccess)
                {
                    jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    IsSuccess = !string.IsNullOrEmpty(jsonResponse);
                }

                if (IsSuccess)
                {
                    imageSearchResults = JsonConvert.DeserializeObject<ImageSearchResults>(jsonResponse)!;
                    IsSuccess = (imageSearchResults != null) &&
                        (imageSearchResults.Results != null) &&
                        imageSearchResults.Results.Any();
                }

                if (IsSuccess)
                {
                    foreach (ImageSearchResult result in imageSearchResults.Results)
                    {
                        Image image = new Image()
                        {
                            ID = result.id,
                            AltDescription = result.alt_description,
                            DownloadUrl = new Uri(result.links.download),
                            RegularUrl = new Uri(result.urls.regular)
                        };
                        images.Add(image);
                    }
                }
            }

            return images;
        }

        public async Task<byte[]> DownloadImageAsync(Uri downloadUrl)
        {
            byte[] image = await _httpClient.GetByteArrayAsync(downloadUrl);
            return image;
        }
    }

    /* Classes needed to deserialize  Json response. 
     https://json2csharp.com/ */

    public class ImageSearchResults
    {
        [JsonProperty(PropertyName = "results")]
        public List<ImageSearchResult> Results { get; set; }
    }

    public class ImageSearchResult
    {
        public string id { get; set; }
        public string alt_description { get; set; }
        public Urls urls { get; set; }
        public Links links { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int likes { get; set; }
    }

    public class Urls
    {
        public string regular { get; set; }
    }

    public class Links
    {
        public string download { get; set; }
    }

}
