# Semantic Kernel Demo Playground

A small, focused playground for **Microsoft Semantic Kernel** using an OpenAI‑compatible endpoint (Groq, OpenAI, local, etc.).

You can pick from multiple demos at runtime and see common patterns: prompt calls, streaming, tools, and prompt plugins.

## Quick Start

1) Set your environment values (or create a `.env` file):

```bash
LLM_BASE_URL=https://api.groq.com/openai/v1
LLM_API_KEY=your_key_here
LLM_MODEL=llama-3.3-70b-versatile
```

2) Run the CLI:

```bash
dotnet run
```

You’ll see a menu and can select which demo to run.

## Demo Menu

Current demos you can launch from the CLI:

1. **Basic prompt** — a simple single‑shot prompt.
2. **Chat with tools** — uses `EmailPlannerPlugin` with `AutoInvokeKernelFunctions`.
3. **Prompt plugin (Joke)** — loads prompt plugins from `KernelPlugins/Prompt`.
4. **Streaming response** — streams content from a prompt function.
5. **Multi‑turn chat with summary** — runs a small conversation and summarizes it using the Summarizer prompt.

You can also run a demo directly by passing the demo number:

```bash
dotnet run -- 3
```

## Project Layout

```
KernelBuilders/           # Custom HTTP handler + builder
KernelPlugins/            # Native and prompt plugins
Demos/                    # Demo catalog + CLI menu
Program.cs                # Entry point
```

## Configuration Details

The app reads these settings from environment variables or a `.env` file (no clobbering of existing env vars):

- `LLM_BASE_URL`
- `LLM_API_KEY`
- `LLM_MODEL`

If any are missing, the app stops with a clear error message.

## Tips and Next Steps

Here are some easy ways to extend this demo:

- **Add a new demo**: create a new runner in `Demos/DemoCatalog.cs` and register it in `All`.
- **Add a new prompt plugin**: drop a folder with `skprompt.txt` + `config.json` into `KernelPlugins/Prompt`.
- **Try a different model/provider**: swap `LLM_BASE_URL` and `LLM_MODEL` in `.env`.
- **Add streaming + tool calls together**: combine demos 2 and 4 into a hybrid workflow.
- **Capture outputs**: write demo results to files for quick comparisons.

