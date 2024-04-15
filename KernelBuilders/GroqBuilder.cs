using System.Net.Http.Headers;
using Microsoft.SemanticKernel;

namespace demo_sk.KernelBuilders;

sealed class GroqRedirectingHandler(string apiKey, bool debug = false) : DelegatingHandler(new HttpClientHandler())
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        request.RequestUri = new Uri("https://api.groq.com/openai/v1/chat/completions");

        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

        if (!debug) return await base.SendAsync(request, cancellationToken);
        PrintRequestHeaders(request);
        Console.WriteLine("Sending request to URL: " + request.RequestUri);

        return await base.SendAsync(request, cancellationToken);
    }

    private void PrintRequestHeaders(HttpRequestMessage request)
    {
        Console.WriteLine("Request Headers:");
        foreach (var header in request.Headers)
        {
            Console.WriteLine($"{header.Key}: {string.Join(", ", header.Value)}");
        }

        if (request.Content == null) return;
        {
            Console.WriteLine("Content Headers:");
            foreach (var header in request.Content.Headers)
            {
                Console.WriteLine($"{header.Key}: {string.Join(", ", header.Value)}");
            }
        }
    }
}

public class GroqBuilder
{
    // Private constructor to prevent instantiation with 'new'
    private GroqBuilder()
    {
    }

    // Public static method to instantiate CroqBuilder
    public static IKernelBuilder Create(string apiKey, string model = "mixtral-8x7b-32768")
    {
        var handler = new GroqRedirectingHandler(apiKey);
        var client = new HttpClient(handler);

        return Kernel
            .CreateBuilder()
            .AddOpenAIChatCompletion(modelId: model, apiKey: apiKey, httpClient: client);
    }
}