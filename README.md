# AgenticAI

This repository contains a simple C# project that demonstrates how to generate automated UI and API test scripts using OpenAI's language model.

## Projects

- **AutomatedTesting.sln** – Solution file containing the `OpenAITestGenerator` console application.
- **OpenAITestGenerator** – Console application that calls the OpenAI API to generate C# test code.

## Usage

1. Install the .NET 8 SDK and ensure `dotnet` is available.
2. Set the environment variable `OPENAI_API_KEY` with your OpenAI API key.
3. Build and run the console application:

   ```bash
   dotnet run --project OpenAITestGenerator
   ```

4. When prompted, enter a description of the UI or API scenario you want tests for. The generated C# test script will be displayed and saved to `GeneratedTest.cs`.
