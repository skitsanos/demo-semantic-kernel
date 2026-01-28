using Microsoft.SemanticKernel;

namespace demo_sk.Demos;

public sealed record DemoDefinition(string Id, string Title, Func<Kernel, Task> RunAsync);
