// See https://aka.ms/new-console-template for more information

using GedankRayze.KernelBuilders;
using demo_sk.Demos;
using Microsoft.SemanticKernel;

DotNetEnv.Env.NoClobber().Load();

var baseUrl = Environment.GetEnvironmentVariable("LLM_BASE_URL");
var apiKey = Environment.GetEnvironmentVariable("LLM_API_KEY");
var model = Environment.GetEnvironmentVariable("LLM_MODEL");

if (string.IsNullOrWhiteSpace(baseUrl) ||
    string.IsNullOrWhiteSpace(apiKey) ||
    string.IsNullOrWhiteSpace(model))
{
    throw new InvalidOperationException(
        "Missing LLM configuration. Ensure LLM_BASE_URL, LLM_API_KEY, and LLM_MODEL are set in the environment or a .env file.");
}

var builder = CustomBuilder.ChatCompletion(baseUrl: baseUrl, apiKey, model: model);
var kernel = builder.Build();

var selection = DemoMenu.PromptForSelection(args, DemoCatalog.All);
if (selection is null)
{
    return;
}

var demo = DemoCatalog.All[selection.Value];
Console.WriteLine($"Running: {demo.Title}\n");
await demo.RunAsync(kernel);
