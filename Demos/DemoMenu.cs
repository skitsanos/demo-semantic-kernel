namespace demo_sk.Demos;

public static class DemoMenu
{
    public static int? PromptForSelection(string[] args, IReadOnlyList<DemoDefinition> demos)
    {
        if (args.Length > 0)
        {
            var arg = args[0].Trim();
            var resolved = ResolveSelection(arg, demos);
            if (resolved is not null)
            {
                return resolved;
            }

            Console.WriteLine("Invalid selection provided in args. Use a number or demo id.");
            return null;
        }

        Console.WriteLine("Select a demo:");
        for (var i = 0; i < demos.Count; i++)
        {
            var demo = demos[i];
            Console.WriteLine($"  {i + 1}. {demo.Title}  (id: {demo.Id})");
        }
        Console.WriteLine("  q. Quit");

        while (true)
        {
            Console.Write("\nEnter choice: ");
            var input = Console.ReadLine();
            if (input is null)
            {
                return null;
            }

            input = input.Trim();
            if (string.Equals(input, "q", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            var selection = ResolveSelection(input, demos);
            if (selection is not null)
            {
                return selection;
            }

            Console.WriteLine("Invalid choice. Enter a number, demo id, or q.");
        }
    }

    private static int? ResolveSelection(string input, IReadOnlyList<DemoDefinition> demos)
    {
        if (int.TryParse(input, out var index))
        {
            index -= 1;
            if (index >= 0 && index < demos.Count)
            {
                return index;
            }

            return null;
        }

        for (var i = 0; i < demos.Count; i++)
        {
            if (string.Equals(demos[i].Id, input, StringComparison.OrdinalIgnoreCase))
            {
                return i;
            }
        }

        return null;
    }
}
