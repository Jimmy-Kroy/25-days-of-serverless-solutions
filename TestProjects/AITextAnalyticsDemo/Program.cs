using Azure;
using Azure.AI.TextAnalytics;
using Microsoft.Extensions.Configuration;

namespace AITextAnalyticsDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {

                var config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true)
                    .Build();

                var endpoint = config["endpoint"];
                var apiKey = config["ApiKey"];

                Console.WriteLine("App started!");
                Console.WriteLine($"Config: {endpoint}, {apiKey}");

                Uri uri = new(endpoint);
                AzureKeyCredential credential = new(apiKey);
                TextAnalyticsClient client = new(uri, credential);

                Console.WriteLine("Calling DetectLanguage!");
                DetectLanguage(client);

                Console.WriteLine("Calling AnalyzeSentiment!");
                AnalyzeSentiment(client);

                Console.WriteLine("Calling ExtractKeyPhrases!");
                ExtractKeyPhrases(client);

                Console.WriteLine("Calling RecognizeNamedEntities!");
                RecognizeNamedEntities(client);

                Console.WriteLine("Calling RecognizePIIEntities!");
                RecognizePIIEntities(client);

                Console.WriteLine("Calling RecognizeLinkedEntities!");
                RecognizeLinkedEntities(client);

                Console.WriteLine("Calling DetectLanguageAsynchronously!");
                await DetectLanguageAsynchronously(client);

                Console.WriteLine("Calling RecognizeNamedEntitiesAsynchronously!");
                await RecognizeNamedEntitiesAsynchronously(client);

                Console.WriteLine("App Finished!");
            }
            catch (RequestFailedException exception)
            {
                Console.WriteLine($"Error Code: {exception.ErrorCode}");
                Console.WriteLine($"Message: {exception.Message}");
            }
        }
        private static void DetectLanguage(TextAnalyticsClient client)
        {
            string document =
                "Este documento está escrito en un lenguaje diferente al inglés. Su objectivo es demostrar cómo"
                + " invocar el método de Detección de Lenguaje del servicio de Text Analytics en Microsoft Azure."
                + " También muestra cómo acceder a la información retornada por el servicio. Esta funcionalidad es"
                + " útil para los sistemas de contenido que recopilan texto arbitrario, donde el lenguaje no se conoce"
                + " de antemano. Puede usarse para detectar una amplia gama de lenguajes, variantes, dialectos y"
                + " algunos idiomas regionales o culturales.";

            try
            {
                Response<DetectedLanguage> response = client.DetectLanguage(document);
                DetectedLanguage language = response.Value;

                Console.WriteLine($"Detected language is {language.Name} with a confidence score of {language.ConfidenceScore}.");
            }
            catch (RequestFailedException exception)
            {
                Console.WriteLine($"Error Code: {exception.ErrorCode}");
                Console.WriteLine($"Message: {exception.Message}");
            }
        }

        private static void AnalyzeSentiment(TextAnalyticsClient client)
        {
            string document =
                "I had the best day of my life. I decided to go sky-diving and it made me appreciate my whole life so"
                + "much more. I developed a deep-connection with my instructor as well, and I feel as if I've made a"
                + "life-long friend in her.";

            try
            {
                Response<DocumentSentiment> response = client.AnalyzeSentiment(document);
                DocumentSentiment docSentiment = response.Value;

                Console.WriteLine($"Document sentiment is {docSentiment.Sentiment} with: ");
                Console.WriteLine($"  Positive confidence score: {docSentiment.ConfidenceScores.Positive}");
                Console.WriteLine($"  Neutral confidence score: {docSentiment.ConfidenceScores.Neutral}");
                Console.WriteLine($"  Negative confidence score: {docSentiment.ConfidenceScores.Negative}");
            }
            catch (RequestFailedException exception)
            {
                Console.WriteLine($"Error Code: {exception.ErrorCode}");
                Console.WriteLine($"Message: {exception.Message}");
            }
        }

        private static void ExtractKeyPhrases(TextAnalyticsClient client)
        {
            string document =
                "My cat might need to see a veterinarian. It has been sneezing more than normal, and although my"
                + " little sister thinks it is funny, I am worried it has the cold that I got last week. We are going"
                + " to call tomorrow and try to schedule an appointment for this week. Hopefully it will be covered by"
                + " the cat's insurance. It might be good to not let it sleep in my room for a while.";

            try
            {
                Response<KeyPhraseCollection> response = client.ExtractKeyPhrases(document);
                KeyPhraseCollection keyPhrases = response.Value;

                Console.WriteLine($"Extracted {keyPhrases.Count} key phrases:");
                foreach (string keyPhrase in keyPhrases)
                {
                    Console.WriteLine($"  {keyPhrase}");
                }
            }
            catch (RequestFailedException exception)
            {
                Console.WriteLine($"Error Code: {exception.ErrorCode}");
                Console.WriteLine($"Message: {exception.Message}");
            }
        }

        private static void RecognizeNamedEntities(TextAnalyticsClient client)
        {
            string document =
                "We love this trail and make the trip every year. The views are breathtaking and well worth the hike!"
                + " Yesterday was foggy though, so we missed the spectacular views. We tried again today and it was"
                + " amazing. Everyone in my family liked the trail although it was too challenging for the less"
                + " athletic among us. Not necessarily recommended for small children. A hotel close to the trail"
                + " offers services for childcare in case you want that.";

            try
            {
                Response<CategorizedEntityCollection> response = client.RecognizeEntities(document);
                CategorizedEntityCollection entitiesInDocument = response.Value;

                Console.WriteLine($"Recognized {entitiesInDocument.Count} entities:");
                foreach (CategorizedEntity entity in entitiesInDocument)
                {
                    Console.WriteLine($"  Text: {entity.Text}");
                    Console.WriteLine($"  Offset: {entity.Offset}");
                    Console.WriteLine($"  Length: {entity.Length}");
                    Console.WriteLine($"  Category: {entity.Category}");
                    if (!string.IsNullOrEmpty(entity.SubCategory))
                        Console.WriteLine($"  SubCategory: {entity.SubCategory}");
                    Console.WriteLine($"  Confidence score: {entity.ConfidenceScore}");
                    Console.WriteLine();
                }
            }
            catch (RequestFailedException exception)
            {
                Console.WriteLine($"Error Code: {exception.ErrorCode}");
                Console.WriteLine($"Message: {exception.Message}");
            }
        }

        /*
         * Run a predictive model to identify a collection of entities containing Personally Identifiable Information 
         * found in the passed-in document or batch of documents, and categorize those entities into categories such 
         * as US social security number, drivers license number, or credit card number.
         */
        private static void RecognizePIIEntities(TextAnalyticsClient client)
        {
            string document =
                "Parker Doe has repaid all of their loans as of 2020-04-25. Their SSN is 859-98-0987. To contact them,"
                + " use their phone number 800-102-1100. They are originally from Brazil and have document ID number"
                + " 998.214.865-68.";

            try
            {
                Response<PiiEntityCollection> response = client.RecognizePiiEntities(document);
                PiiEntityCollection entities = response.Value;

                Console.WriteLine($"Redacted Text: {entities.RedactedText}");
                Console.WriteLine();
                Console.WriteLine($"Recognized {entities.Count} PII entities:");
                foreach (PiiEntity entity in entities)
                {
                    Console.WriteLine($"  Text: {entity.Text}");
                    Console.WriteLine($"  Category: {entity.Category}");
                    if (!string.IsNullOrEmpty(entity.SubCategory))
                        Console.WriteLine($"  SubCategory: {entity.SubCategory}");
                    Console.WriteLine($"  Confidence score: {entity.ConfidenceScore}");
                    Console.WriteLine();
                }
            }
            catch (RequestFailedException exception)
            {
                Console.WriteLine($"Error Code: {exception.ErrorCode}");
                Console.WriteLine($"Message: {exception.Message}");
            }
        }

        private static void RecognizeLinkedEntities(TextAnalyticsClient client)
        {
            string document =
                "Microsoft was founded by Bill Gates with some friends he met at Harvard. One of his friends, Steve"
                + " Ballmer, eventually became CEO after Bill Gates as well. Steve Ballmer eventually stepped down as"
                + " CEO of Microsoft, and was succeeded by Satya Nadella. Microsoft originally moved its headquarters"
                + " to Bellevue, Washington in Januaray 1979, but is now headquartered in Redmond.";

            try
            {
                Response<LinkedEntityCollection> response = client.RecognizeLinkedEntities(document);
                LinkedEntityCollection linkedEntities = response.Value;

                Console.WriteLine($"Recognized {linkedEntities.Count} entities:");
                foreach (LinkedEntity linkedEntity in linkedEntities)
                {
                    Console.WriteLine($"  Name: {linkedEntity.Name}");
                    Console.WriteLine($"  Language: {linkedEntity.Language}");
                    Console.WriteLine($"  Data Source: {linkedEntity.DataSource}");
                    Console.WriteLine($"  URL: {linkedEntity.Url}");
                    Console.WriteLine($"  Entity Id in Data Source: {linkedEntity.DataSourceEntityId}");
                    foreach (LinkedEntityMatch match in linkedEntity.Matches)
                    {
                        Console.WriteLine($"    Match Text: {match.Text}");
                        Console.WriteLine($"    Offset: {match.Offset}");
                        Console.WriteLine($"    Length: {match.Length}");
                        Console.WriteLine($"    Confidence score: {match.ConfidenceScore}");
                    }
                    Console.WriteLine();
                }
            }
            catch (RequestFailedException exception)
            {
                Console.WriteLine($"Error Code: {exception.ErrorCode}");
                Console.WriteLine($"Message: {exception.Message}");
            }
        }

        private static async Task DetectLanguageAsynchronously(TextAnalyticsClient client)
        {
            string document =
                "Este documento está escrito en un lenguaje diferente al inglés. Su objectivo es demostrar cómo"
                + " invocar el método de Detección de Lenguaje del servicio de Text Analytics en Microsoft Azure."
                + " También muestra cómo acceder a la información retornada por el servicio. Esta funcionalidad es"
                + " útil para los sistemas de contenido que recopilan texto arbitrario, donde el lenguaje no se conoce"
                + " de antemano. Puede usarse para detectar una amplia gama de lenguajes, variantes, dialectos y"
                + " algunos idiomas regionales o culturales.";

            try
            {
                Response<DetectedLanguage> response = await client.DetectLanguageAsync(document);
                DetectedLanguage language = response.Value;

                Console.WriteLine($"Detected language is {language.Name} with a confidence score of {language.ConfidenceScore}.");
            }
            catch (RequestFailedException exception)
            {
                Console.WriteLine($"Error Code: {exception.ErrorCode}");
                Console.WriteLine($"Message: {exception.Message}");
            }
        }

        private static async Task RecognizeNamedEntitiesAsynchronously(TextAnalyticsClient client)
        {
            string document =
                "We love this trail and make the trip every year. The views are breathtaking and well worth the hike!"
                + " Yesterday was foggy though, so we missed the spectacular views. We tried again today and it was"
                + " amazing. Everyone in my family liked the trail although it was too challenging for the less"
                + " athletic among us. Not necessarily recommended for small children. A hotel close to the trail"
                + " offers services for childcare in case you want that.";

            try
            {
                Response<CategorizedEntityCollection> response = await client.RecognizeEntitiesAsync(document);
                CategorizedEntityCollection entitiesInDocument = response.Value;

                Console.WriteLine($"Recognized {entitiesInDocument.Count} entities:");
                foreach (CategorizedEntity entity in entitiesInDocument)
                {
                    Console.WriteLine($"  Text: {entity.Text}");
                    Console.WriteLine($"  Offset: {entity.Offset}");
                    Console.WriteLine($"  Length: {entity.Length}");
                    Console.WriteLine($"  Category: {entity.Category}");
                    if (!string.IsNullOrEmpty(entity.SubCategory))
                        Console.WriteLine($"  SubCategory: {entity.SubCategory}");
                    Console.WriteLine($"  Confidence score: {entity.ConfidenceScore}");
                    Console.WriteLine();
                }
            }
            catch (RequestFailedException exception)
            {
                Console.WriteLine($"Error Code: {exception.ErrorCode}");
                Console.WriteLine($"Message: {exception.Message}");
            }
        }
    }
}