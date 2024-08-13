using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WishListApp.Configurations;
using WishListApp.Models;

namespace WishListApp.Services
{
    public class SlackClient<T> : ISlackClient<T>
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SlackClient<T>> _logger;
        private readonly SlackSettings _slackSettings;

        public SlackClient(HttpClient httpClient, IOptions<SlackSettings> slackSettings, ILogger<SlackClient<T>> logger)
        {
            _httpClient = httpClient;
            _slackSettings = slackSettings.Value;
            _logger = logger;
        }

        public async Task<bool> PostAsync(T item)
        {
            bool IsSuccess = false;

            StringContent? content = FormatMessage(item);

            IsSuccess = (content != null);

            if (IsSuccess)
            {
                HttpResponseMessage response =
                    await _httpClient.PostAsync(_slackSettings.EndpointUrl, content);

                IsSuccess = response.IsSuccessStatusCode;
            }

            return IsSuccess;
        }

        private StringContent? FormatMessage(T item)
        {
            bool IsSuccess = false;
            StringContent? content = null;
            string msg = string.Empty;

            Wish? wish = item as Wish;
            IsSuccess = (wish != null);

            if (IsSuccess)
            {
                msg = $"New wish rcvd: who[{wish.Who}], description[{wish.Description}], address[{wish.Address}].";
                string jsonString = JsonConvert.SerializeObject(new
                {
                    text = msg
                });

                content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            }

            return IsSuccess == true ? content : null;
        }
    }
}
