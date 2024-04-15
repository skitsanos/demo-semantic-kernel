using System.Net.Http.Headers;
using Microsoft.SemanticKernel;

namespace GedankRayze.KernelBuilders
{
    sealed class CustomRedirectingHandler(string baseUrl, string apiKey)
        : DelegatingHandler(new HttpClientHandler())
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            request.RequestUri = new Uri($"{baseUrl}/chat/completions");

            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

            return await base.SendAsync(request, cancellationToken);
        }
    }

    public class CustomBuilder
    {
        // Private constructor to prevent instantiation with 'new'
        private CustomBuilder()
        {
        }

        // Public static method to instantiate CroqBuilder
        public static IKernelBuilder ChatCompletion(
            string baseUrl,
            string apiKey,
            string model,
            string? serviceId = null
        )
        {
            var handler = new CustomRedirectingHandler(baseUrl, apiKey);
            var client = new HttpClient(handler);

            return Kernel
                .CreateBuilder()
                .AddOpenAIChatCompletion(modelId: model, apiKey: apiKey, httpClient: client, serviceId: serviceId);
        }
    }
}