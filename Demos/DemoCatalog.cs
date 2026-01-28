using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace demo_sk.Demos;

public static class DemoCatalog
{
    public static readonly IReadOnlyList<DemoDefinition> All = new List<DemoDefinition>
    {
        new("1", "Basic prompt", BasicPromptAsync),
        new("2", "Chat with tools (EmailPlannerPlugin)", ChatWithToolsAsync),
        new("3", "Prompt plugin (Joke)", PromptPluginAsync),
        new("4", "Streaming response", StreamingResponseAsync),
        new("5", "Multi-turn chat with summary", MultiTurnWithSummaryAsync)
    };

    private static Task BasicPromptAsync(Kernel kernel)
    {
        var template = $"""
                        I am in Lisbon right now. Today is {DateTime.Now:D}. I want to write an email to my boss that I want to move here.
                        """;

        return RunPromptAsync(kernel, template);
    }

    private static async Task RunPromptAsync(Kernel kernel, string template)
    {
        var result = await kernel.InvokePromptAsync<string>(template);
        Console.WriteLine(result);
    }

    private static async Task ChatWithToolsAsync(Kernel kernel)
    {
        kernel.Plugins.AddFromType<EmailPlannerPlugin>();

        var chatMessages = new ChatHistory("""
                                           You are a friendly assistant who likes to follow the rules. You will complete required steps
                                           and request approval before taking any consequential actions. If the user doesn't provide
                                           enough information for you to complete a task, you will keep asking questions until you have
                                           enough information to complete the task.
                                           """);

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
        Console.WriteLine();
    }

    private static async Task PromptPluginAsync(Kernel kernel)
    {
        var prompts = kernel.ImportPluginFromPromptDirectory(pluginDirectory: DemoPaths.PromptPluginDirectory);

        var kernelArgs = new KernelArguments
        {
            { "input", "short poetry" },
            { "style", "Horror" }
        };

        var chatResult = kernel.InvokeStreamingAsync<StreamingChatMessageContent>(
            prompts["Joke"],
            arguments: kernelArgs
        );

        await foreach (var chunk in chatResult)
        {
            if (chunk.Role.HasValue)
            {
                Console.Write(chunk.Role + " > ");
            }

            Console.Write(chunk);
        }

        Console.WriteLine();
    }

    private static async Task StreamingResponseAsync(Kernel kernel)
    {
        var functionFromPrompt = kernel.CreateFunctionFromPrompt(
            "Create a joke about {{$input}} in this style: {{$style}}",
            functionName: "joke_generator");

        var kernelArgs = new KernelArguments
        {
            { "input", "getting stuck in Lisbon" },
            { "style", "Italian opera" }
        };

        var messages = kernel.InvokeStreamingAsync(function: functionFromPrompt, arguments: kernelArgs);

        await foreach (var content in messages)
        {
            Console.Write(content);
        }

        Console.WriteLine();
    }

    private static async Task MultiTurnWithSummaryAsync(Kernel kernel)
    {
        var chatHistory = new ChatHistory("You are a concise, helpful assistant.");
        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

        chatHistory.AddUserMessage("I’m moving to Lisbon next month and need to notify my team.");
        var first = await chatCompletionService.GetChatMessageContentsAsync(chatHistory);
        var firstReply = first.FirstOrDefault();
        if (firstReply?.Content is { Length: > 0 })
        {
            chatHistory.AddAssistantMessage(firstReply.Content);
            Console.WriteLine(firstReply.Content);
        }

        chatHistory.AddUserMessage("Also mention I’ll still be available for meetings in the mornings.");
        var second = await chatCompletionService.GetChatMessageContentsAsync(chatHistory);
        var secondReply = second.FirstOrDefault();
        if (secondReply?.Content is { Length: > 0 })
        {
            chatHistory.AddAssistantMessage(secondReply.Content);
            Console.WriteLine(secondReply.Content);
        }

        var prompts = kernel.ImportPluginFromPromptDirectory(pluginDirectory: DemoPaths.PromptPluginDirectory);
        var conversationText = string.Join("\n", chatHistory.Select(message => $"{message.Role}: {message.Content}"));

        var summaryResult = await kernel.InvokeAsync(
            prompts["Summarizer"],
            new KernelArguments { { "input", conversationText } }
        );

        Console.WriteLine("\nSummary:");
        Console.WriteLine(summaryResult);
    }
}
