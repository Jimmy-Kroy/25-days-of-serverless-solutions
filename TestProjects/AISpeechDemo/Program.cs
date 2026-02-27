using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

//Reference
//Code examples can be found here:
//https://learn.microsoft.com/en-us/azure/ai-services/speech-service/


namespace AISpeechDemo
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

                Console.WriteLine("Calling speech synthesis.");
                var speechConfig = SpeechConfig.FromEndpoint(new Uri(endpoint), apiKey);

                //You can choose voices and languages from following url
                //https://learn.microsoft.com/en-us/azure/ai-services/speech-service/language-support?tabs=tts#standard-voices
                // The neural multilingual voice can speak different languages based on the input text.
                speechConfig.SpeechSynthesisVoiceName = "en-US-Ava:DragonHDLatestNeural";
                //speechConfig.SpeechSynthesisVoiceName = "nl-NL-ColetteNeural";
                //speechConfig.SpeechSynthesisVoiceName = "es-ES-Ximena:DragonHDLatestNeural";

                using (var speechSynthesizer = new SpeechSynthesizer(speechConfig))
                {
                    // Get text from the console and synthesize to the default speaker.
                    //Console.WriteLine("Enter some text that you want to speak >");
                    //string text = Console.ReadLine();
                    DateTime now = DateTime.Now;
                    string text = $"The time is {now.Hour}:{now.Minute:D2}";
                    Console.WriteLine(text);

                    var speechSynthesisResult = await speechSynthesizer.SpeakTextAsync(text);
                    OutputSpeechSynthesisResult(speechSynthesisResult, text);
                }

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();

                Console.WriteLine("Calling speech to text conversion.");
                speechConfig = SpeechConfig.FromEndpoint(new Uri(endpoint), apiKey);
                speechConfig.SpeechRecognitionLanguage = "en-US";

                //If you want to use the default Microphone  
                //using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
                //If you want to use a file instead of Microphone   
                using var audioConfig = AudioConfig.FromWavFileInput("Labfiles_07-speech_Python_speaking-clock_time.wav");
                using var speechRecognizer = new SpeechRecognizer(speechConfig, audioConfig);

                Console.WriteLine("Speak into your microphone.");
                var speechRecognitionResult = await speechRecognizer.RecognizeOnceAsync();
                OutputSpeechRecognitionResult(speechRecognitionResult);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Message: {exception.Message}");
            }
        }

        static void OutputSpeechSynthesisResult(SpeechSynthesisResult speechSynthesisResult, string text)
        {
            switch (speechSynthesisResult.Reason)
            {
                case ResultReason.SynthesizingAudioCompleted:
                    Console.WriteLine($"Speech synthesized for text: [{text}]");
                    break;
                case ResultReason.Canceled:
                    var cancellation = SpeechSynthesisCancellationDetails.FromResult(speechSynthesisResult);
                    Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                        Console.WriteLine($"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");
                        Console.WriteLine($"CANCELED: Did you set the speech resource key and endpoint values?");
                    }
                    break;
                default:
                    break;
            }
        }

        static void OutputSpeechRecognitionResult(SpeechRecognitionResult speechRecognitionResult)
        {
            switch (speechRecognitionResult.Reason)
            {
                case ResultReason.RecognizedSpeech:
                    Console.WriteLine($"RECOGNIZED: Text={speechRecognitionResult.Text}");
                    break;
                case ResultReason.NoMatch:
                    Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                    break;
                case ResultReason.Canceled:
                    var cancellation = CancellationDetails.FromResult(speechRecognitionResult);
                    Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                        Console.WriteLine($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                        Console.WriteLine($"CANCELED: Did you set the speech resource key and endpoint values?");
                    }
                    break;
            }
        }
    }
}