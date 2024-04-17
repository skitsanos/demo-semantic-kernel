// See https://aka.ms/new-console-template for more information

using GedankRayze.KernelBuilders;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

var builder = CustomBuilder.ChatCompletion(
    baseUrl: "https://api.groq.com/openai/v1",
    Environment.GetEnvironmentVariable("LLM_API_KEY") ?? string.Empty,
    model: "mixtral-8x7b-32768"
);

//https://www.codemag.com/Article/2403061/Semantic-Kernel-101-Part-2

var chatMessages = new ChatHistory("""
                                   You are a friendly assistant who likes to follow the rules. You will complete required steps
                                   and request approval before taking any consequential actions. If the user doesn't provide
                                   enough information for you to complete a task, you will keep asking questions until you have
                                   enough information to complete the task.
                                   """);

builder.Plugins.AddFromType<EmailPlannerPlugin>();

var kernel = builder.Build();

chatMessages.AddUserMessage("""
                            I want to write an email to Gedank Rayze about me moving to Lisbon, sign it with your name.
                            """);


var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

OpenAIPromptExecutionSettings execSettings = new()
{
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
};

var chat = await chatCompletionService.GetChatMessageContentsAsync(
    chatMessages,
    execSettings,
    kernel
);

foreach (var content in chat) Console.Write(content.Content);

// var msgs = await chatCompletionService.GetChatMessageContentsAsync(
//     chatHistory: chatMessages,
//     executionSettings: execSettings);
// foreach (var content in msgs)
// {
//     Console.WriteLine(content.Role.Label);
//     Console.WriteLine(content.Content);
// }