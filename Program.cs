// See https://aka.ms/new-console-template for more information

using demo_sk.KernelBuilders;
using GedankRayze.KernelBuilders;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

var builder = CustomBuilder.ChatCompletion(
    baseUrl: "https://api.groq.com/openai/v1",
    apiKey: "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",
    model: "mixtral-8x7b-32768"
);
//builder.Services.AddLogging(c => c.SetMinimumLevel(LogLevel.Trace).AddDebug());

var chatMessages = new ChatHistory("""
                                   You are a friendly assistant who likes to follow the rules. You will complete required steps
                                   and request approval before taking any consequential actions. If the user doesn't provide
                                   enough information for you to complete a task, you will keep asking questions until you have
                                   enough information to complete the task.
                                   """);

builder.Plugins.AddFromType<EmailPlannerPlugin>();

var kernel = builder.Build();

var folderWithPlugins =
    "/Users/skitsanos/FTP/Projects/dotnet/demo-sk/KernelPlugins";
//Path.Join(AppDomain.CurrentDomain.BaseDirectory, "KernelPlugins");

//kernel.ImportPluginFromPromptDirectory(pluginDirectory: folderWithPlugins, "Summarizer");
kernel.ImportPluginFromPromptDirectory(pluginDirectory: folderWithPlugins);

//I am in Lisbon right now. Tell me the season of the year for this date and then summarize in one word: {DateTime.Now:D}

var template =
    $"""
     I am in Lisbon right now. Today is {DateTime.Now:D}. I want to write an email to my boss (info@skitsanos.com) that i want to move here.
     Summarize the climate for today and offer to review summary.
     """;
//Be creative about the subject of email.""";

// var currentSeason = await kernel.InvokePromptAsync<string>(template);
//
// Console.WriteLine(currentSeason);

chatMessages.AddUserMessage(template);

IChatCompletionService chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();


OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
{
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
};

var msgs = await chatCompletionService.GetChatMessageContentsAsync(chatMessages,
    executionSettings: openAIPromptExecutionSettings);
foreach (var content in msgs)
{
    Console.WriteLine(content.Role.Label);
    Console.WriteLine(content.Content);
}

// Console.WriteLine("--- Writing a poem");
//
// const string prompt = "Write a short poem about cats";
// var response = kernel.InvokePromptStreamingAsync(prompt);
// await foreach (var message in response)
// {
//     Console.Write(message);
// }