# AgenticAI

This repository contains a simple C# project that demonstrates how to generate automated UI and API test scripts using OpenAI's language model.

## Projects

- **AutomatedTesting.sln** – Solution file containing the `OpenAITestGenerator` console application.
- **OpenAITestGenerator** – Console application that calls the OpenAI API to generate C# test code.
- **GeneratedTests** – xUnit test project where OpenAI generates test files.
- **WebTestExecutor** – Console application that executes AI-generated web test steps using Selenium WebDriver.

## Usage

1. Install the .NET 8 SDK and ensure `dotnet` is available.
2. Set the environment variable `OPENAI_API_KEY` with your OpenAI API key.
3. Build and run the console application:

   ```bash
   dotnet run --project OpenAITestGenerator
   ```
4. When the program starts you will be asked to either upload a user story or select an existing one from the `UserStories` folder. Selecting a story will trigger test generation and attempt to run the generated tests inside the `GeneratedTests` project. The application will create the `UserStories` folder automatically if it does not exist.
5. To execute web scenarios, build and run the WebTestExecutor project with a scenario description as argument:

   ```bash
   dotnet run --project WebTestExecutor "Login scenario"
   ```
