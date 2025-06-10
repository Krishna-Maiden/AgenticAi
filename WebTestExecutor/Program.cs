using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OpenAI_API;
using OpenAI_API.Completions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace WebTestExecutor
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: WebTestExecutor <scenario>");
                return;
            }

            string scenario = string.Join(" ", args);
            string apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                Console.Error.WriteLine("OPENAI_API_KEY environment variable is not set.");
                return;
            }

            var api = new OpenAIAPI(apiKey);
            var request = new CompletionRequest
            {
                Prompt = $"Generate step by step instructions to test the following scenario using a web browser:\n{scenario}\nProvide one instruction per line.",
                Model = OpenAI_API.Models.Model.DavinciText,
                MaxTokens = 150
            };

            Console.WriteLine("Generating test steps with OpenAI...");
            var result = await api.Completions.CreateCompletionAsync(request);
            string stepsText = result.ToString();
            var steps = stepsText.Split(new[] {'\n'}, StringSplitOptions.RemoveEmptyEntries)
                                  .Select(s => s.TrimStart('-',' ')).ToArray();

            Console.WriteLine("Executing steps...");
            using var driver = new ChromeDriver();
            Directory.CreateDirectory("Screenshots");

            for (int i = 0; i < steps.Length; i++)
            {
                string step = steps[i];
                Console.WriteLine($"Step {i + 1}: {step}");
                ExecuteStep(driver, step);
                CaptureScreenshot(driver, i + 1);
            }

            Console.WriteLine("Test execution complete.");
            driver.Quit();
        }

        static void ExecuteStep(IWebDriver driver, string step)
        {
            var parts = step.Split(' ');
            if (parts.Length == 0) return;

            switch (parts[0].ToLower())
            {
                case "open":
                    if (parts.Length > 1)
                        driver.Navigate().GoToUrl(parts[1]);
                    break;
                case "click":
                    if (parts.Length > 1)
                        driver.FindElement(By.CssSelector(parts[1])).Click();
                    break;
                case "type":
                    if (parts.Length > 2)
                        driver.FindElement(By.CssSelector(parts[2])).SendKeys(parts[1]);
                    break;
                case "wait":
                    if (parts.Length > 1 && int.TryParse(parts[1], out int ms))
                        Thread.Sleep(ms);
                    break;
                default:
                    Console.WriteLine($"Unknown action: {parts[0]}");
                    break;
            }
        }

        static void CaptureScreenshot(IWebDriver driver, int index)
        {
            try
            {
                var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                string file = Path.Combine("Screenshots", $"step{index:00}.png");
                screenshot.SaveAsFile(file, ScreenshotImageFormat.Png);
                Console.WriteLine($"Saved screenshot: {file}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Screenshot failed: {ex.Message}");
            }
        }
    }
}
