

using Azure;
using Azure.AI.Vision.ImageAnalysis;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;


//Reference code snippets can be found here:
//https://www.nuget.org/packages/Azure.AI.Vision.ImageAnalysis
//https://microsoftlearning.github.io/mslearn-ai-vision/Instructions/Labs/01-analyze-images.html


namespace AIImageAnalysisDemo
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

                // Create an Image Analysis client.
                ImageAnalysisClient client = new ImageAnalysisClient(new Uri(endpoint), new AzureKeyCredential(apiKey));

                //AnalyzeImage(client, "building.jpg");
                //AnalyzeImage(client, "person.jpg");
                //AnalyzeImage(client, "street.jpg");

                ReadTextInImage(client, "Business-card.jpg");
                ReadTextInImage(client, "Lincoln.jpg");
                ReadTextInImage(client, "Note.jpg");

                Console.WriteLine("App Finished!");

            }
            catch (Exception exception)
            {
                Console.WriteLine($"Message: {exception.Message}");
            }
        }

        private static void AnalyzeImage(ImageAnalysisClient client, string image)
        {

            Console.WriteLine($"Analyzing image: {image}");

            // Use a file stream to pass the image data to the analyze call
            using FileStream stream = new FileStream(image, FileMode.Open);

            var visualFeatures =
                VisualFeatures.Caption |
                VisualFeatures.DenseCaptions |
                VisualFeatures.Objects |
                VisualFeatures.Read |
                VisualFeatures.Tags |
                VisualFeatures.People;


            // Get a caption for the image.
            ImageAnalysisResult result = client.Analyze(
                BinaryData.FromStream(stream),
                visualFeatures, //VisualFeatures.Caption,
                new ImageAnalysisOptions { GenderNeutralCaption = true });


            // Print caption results to the console
            Console.WriteLine($"Image analysis results:");
            Console.WriteLine($" Metadata: Model: {result.ModelVersion} Image dimensions: {result.Metadata.Width} x {result.Metadata.Height}");

            Console.WriteLine($" Caption:");
            Console.WriteLine($"   '{result.Caption.Text}', Confidence {result.Caption.Confidence:F4}");

            Console.WriteLine($" Dense Captions:");
            foreach (DenseCaption denseCaption in result.DenseCaptions.Values)
            {
                Console.WriteLine($"   Region: '{denseCaption.Text}', Confidence {denseCaption.Confidence:F4}, Bounding box {denseCaption.BoundingBox}");
            }

            Console.WriteLine($" Tags:");
            foreach (DetectedTag tag in result.Tags.Values)
            {
                Console.WriteLine($"   '{tag.Name}', Confidence {tag.Confidence:F4}");
            }

            Console.WriteLine($" Objects:");
            foreach (DetectedObject detectedObject in result.Objects.Values)
            {
                Console.WriteLine($"   Object: '{detectedObject.Tags.First().Name}', Bounding box {detectedObject.BoundingBox.ToString()}");
            }

            Console.WriteLine($" People:");
            foreach (DetectedPerson person in result.People.Values)
            {
                Console.WriteLine($"   Person: Bounding box {person.BoundingBox.ToString()}, Confidence {person.Confidence:F4}");
            }
        }

        private static void ReadTextInImage(ImageAnalysisClient client, string image)
        {
            Console.WriteLine($"Analyzing image: {image}");

            // Use a file stream to pass the image data to the analyze call
            using FileStream stream = new FileStream(image, FileMode.Open);

            var visualFeatures = VisualFeatures.Read;

            // Get a caption for the image.
            ImageAnalysisResult result = client.Analyze(
                BinaryData.FromStream(stream),
                visualFeatures,
                new ImageAnalysisOptions { GenderNeutralCaption = true });

            Console.WriteLine($"Image read results:");
            Console.WriteLine($" Metadata: Model: {result.ModelVersion} Image dimensions: {result.Metadata.Width} x {result.Metadata.Height}");
            Console.WriteLine($" Text:");
            foreach (var line in result.Read.Blocks.SelectMany(block => block.Lines))
            {
                Console.WriteLine($"   Line: '{line.Text}', Bounding Polygon: [{string.Join(" ", line.BoundingPolygon)}]");
                foreach (DetectedTextWord word in line.Words)
                {
                    Console.WriteLine($"     Word: '{word.Text}', Confidence {word.Confidence.ToString("#.####")}, Bounding Polygon: [{string.Join(" ", word.BoundingPolygon)}]");
                }
            }
        }
    }
}