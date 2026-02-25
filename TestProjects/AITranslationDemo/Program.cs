using Azure;
using Azure.AI.Translation.Text;
using Newtonsoft.Json;
using System.Net;
using System.Text;


//Reference code samples: https://www.nuget.org/packages/Azure.AI.Translation.Text 

namespace AITranslationDemo
{
    class Program
    {
        private static readonly string key = "djgksfgkdsjcytt356jWq8o5fLrCrbjnbW4pfPcRwlqfmJQQJ99CBACfhMk5XJ3w3AAAbACOGo0Nb";
        private static readonly string endpoint = "https://api.cognitive.microsofttranslator.com";

        // location, also known as region.
        // required if you're using a multi-service or regional (not global) resource. It can be found in the Azure portal on the Keys and Endpoint page.
        private static readonly string location = "Sweden Central";


        static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("App started!");

                string endpoint = "https://api.cognitive.microsofttranslator.com";
                string apiKey = "bgdlajhgjshdg56jWq8o5fLrCrbjnbW4pfPcRwlqfmJQQJ99CBACfhMk5XJ3w3AAAbACOGo0Nb";
                string region = "swedencentral"; // Sweden Central

                TextTranslationClient client = new TextTranslationClient(new AzureKeyCredential(apiKey), new Uri(endpoint), region);

                Console.WriteLine("Calling GetSupportedLanguages:");
                GetSupportedLanguages(client);

                Console.WriteLine("Calling Translate:");
                Translate(client);

                Console.WriteLine("Calling MultiTranslations:");
                MultiTranslations(client);

                Console.WriteLine("App Finished!");
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Message: {exception.Message}");
            }

        }

        private static void GetSupportedLanguages(TextTranslationClient client)
        {
            try
            {
                Response<GetSupportedLanguagesResult> response = client.GetSupportedLanguages(cancellationToken: CancellationToken.None);
                GetSupportedLanguagesResult languages = response.Value;

                Console.WriteLine($"Number of supported languages for translate operations: {languages.Translation.Count}.");
            }
            catch (RequestFailedException exception)
            {
                Console.WriteLine($"Error Code: {exception.ErrorCode}");
                Console.WriteLine($"Message: {exception.Message}");
            }
        }

        private static void Translate(TextTranslationClient client)
        {
            try
            {
                string targetLanguage = "cs";
                string inputText = "This is a test.";

                Response<IReadOnlyList<TranslatedTextItem>> response = client.Translate(targetLanguage, inputText);
                IReadOnlyList<TranslatedTextItem> translations = response.Value;
                TranslatedTextItem translation = translations.FirstOrDefault();

                Console.WriteLine($"Detected languages of the input text: {translation?.DetectedLanguage?.Language} with score: {translation?.DetectedLanguage?.Confidence}.");
                Console.WriteLine($"Text was translated to: '{translation?.Translations?.FirstOrDefault().TargetLanguage}' and the result is: '{translation?.Translations?.FirstOrDefault()?.Text}'.");
            }
            catch (RequestFailedException exception)
            {
                Console.WriteLine($"Error Code: {exception.ErrorCode}");
                Console.WriteLine($"Message: {exception.Message}");
            }
        }

        private static void MultiTranslations(TextTranslationClient client)
        {
            try
            {
                TextTranslationTranslateOptions options = new TextTranslationTranslateOptions(
                    targetLanguages: new[] { "cs", "es", "de" },
                    content: new[] { "This is a test." }
                );

                Response<IReadOnlyList<TranslatedTextItem>> response = client.Translate(options);
                IReadOnlyList<TranslatedTextItem> translations = response.Value;

                foreach (TranslatedTextItem translation in translations)
                {
                    Console.WriteLine($"Detected languages of the input text: {translation?.DetectedLanguage?.Language} with score: {translation?.DetectedLanguage?.Confidence}.");

                    Console.WriteLine($"Text was translated to: '{translation?.Translations?.FirstOrDefault().TargetLanguage}' and the result is: '{translation?.Translations?.FirstOrDefault()?.Text}'.");
                }
            }
            catch (RequestFailedException exception)
            {
                Console.WriteLine($"Error Code: {exception.ErrorCode}");
                Console.WriteLine($"Message: {exception.Message}");
            }
        }

        private static async Task RestMethodCall()
        {
            // Input and output languages are defined as parameters.
            string route = "/translate?api-version=3.0&from=en&to=fr&to=zu";
            string textToTranslate = "I would really like to drive your car around the block a few times!";
            object[] body = new object[] { new { Text = textToTranslate } };
            var requestBody = JsonConvert.SerializeObject(body);

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                // Build the request.
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(endpoint + route);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", key);
                // location required if you're using a multi-service or regional (not global) resource.
                request.Headers.Add("Ocp-Apim-Subscription-Region", location);

                // Send the request and get response.
                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                // Read response as a string.
                string result = await response.Content.ReadAsStringAsync();
                Console.WriteLine(result);
            }

        }
    }
}