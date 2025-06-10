using System;
using System.IO;
using System.Threading.Tasks;
using OpenAI_API;

class Program
{
    static async Task Main(string[] args)
    {
        string apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            Console.Error.WriteLine("Please set the OPENAI_API_KEY environment variable.");
            return;
        }

        Console.WriteLine("Describe the UI or API tests you want to generate:");
        string prompt = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(prompt))
        {
            Console.Error.WriteLine("No prompt provided.");
            return;
        }

        var api = new OpenAIAPI(apiKey);
        var completionRequest = new OpenAI_API.Completions.CompletionRequest()
        {
            Prompt = $"Generate C# automated tests for the following scenario:\n{prompt}",
            Model = OpenAI_API.Models.Model.DavinciText,
            MaxTokens = 400
        };

        Console.WriteLine("Generating test script with OpenAI...");
        var result = await api.Completions.CreateCompletionAsync(completionRequest);
        string text = result.ToString();
        Console.WriteLine("--- Generated Test Script ---\n");
        Console.WriteLine(text);
        File.WriteAllText("GeneratedTest.cs", text);
        Console.WriteLine("Saved to GeneratedTest.cs");
    }
}
