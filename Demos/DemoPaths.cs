namespace demo_sk.Demos;

public static class DemoPaths
{
    public static string PromptPluginDirectory
    {
        get
        {
            var currentDir = Path.Combine(Environment.CurrentDirectory, "KernelPlugins", "Prompt");
            if (Directory.Exists(currentDir))
            {
                return currentDir;
            }

            var baseDir = AppContext.BaseDirectory;
            var fallback = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "KernelPlugins", "Prompt"));
            if (Directory.Exists(fallback))
            {
                return fallback;
            }

            throw new DirectoryNotFoundException("Prompt plugin directory not found. Ensure KernelPlugins/Prompt exists.");
        }
    }
}
