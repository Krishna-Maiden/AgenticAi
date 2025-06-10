using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

        string root = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, ".."));
        string storyDir = Path.Combine(root, "UserStories");
        string testProjDir = Path.Combine(root, "GeneratedTests");
        string testFile = Path.Combine(testProjDir, "GeneratedTest.cs");
        Directory.CreateDirectory(storyDir);
        Directory.CreateDirectory(testProjDir);

        Console.WriteLine("Select an option:");
        Console.WriteLine("1. Upload user story");
        Console.WriteLine("2. Select user story");
        Console.Write("Choice: ");
        string choice = Console.ReadLine();

        string story = string.Empty;

        if (choice == "1")
        {
            Console.Write("Enter a name for the user story: ");
            string name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name))
                name = DateTime.Now.ToString("yyyyMMddHHmmss");
            string path = Path.Combine(storyDir, name + ".txt");
            Console.WriteLine("Enter the user story text:");
            string text = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(text))
            {
                Console.Error.WriteLine("No story provided.");
                return;
            }
            File.WriteAllText(path, text);
            Console.WriteLine($"User story saved to {path}");
            return;
        }
        else if (choice == "2")
        {
            var files = Directory.GetFiles(storyDir, "*.txt");
            if (files.Length == 0)
            {
                Console.WriteLine("No user stories found.");
                return;
            }

            for (int i = 0; i < files.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {Path.GetFileNameWithoutExtension(files[i])}");
            }
            Console.Write("Select story number: ");
            if (!int.TryParse(Console.ReadLine(), out int index) || index < 1 || index > files.Length)
            {
                Console.WriteLine("Invalid selection.");
                return;
            }
            story = File.ReadAllText(files[index - 1]);
        }
        else
        {
            Console.WriteLine("Invalid option.");
            return;
        }

        var api = new OpenAIAPI(apiKey);
        var completionRequest = new OpenAI_API.Completions.CompletionRequest
        {
            Prompt = $"Generate C# xUnit tests for the following user story:\n{story}",
            Model = OpenAI_API.Models.Model.DavinciText,
            MaxTokens = 400
        };

        Console.WriteLine("Generating test script with OpenAI...");
        var result = await api.Completions.CreateCompletionAsync(completionRequest);
        string code = result.ToString();
        File.WriteAllText(testFile, code);
        Console.WriteLine($"Saved to {testFile}");

        try
        {
            var psi = new ProcessStartInfo("dotnet", $"test \"{testProjDir}\"")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };
            using var proc = Process.Start(psi);
            proc.WaitForExit();
            Console.WriteLine(proc.StandardOutput.ReadToEnd());
            Console.Error.Write(proc.StandardError.ReadToEnd());
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("Failed to run tests: " + ex.Message);
        }
    }
}
