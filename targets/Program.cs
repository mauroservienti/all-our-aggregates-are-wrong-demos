using System.IO;
using static Bullseye.Targets;
using static SimpleExec.Command;

internal class Program
{
    public static void Main(string[] args)
    {
        Target(
            "default",
            Directory.EnumerateFiles("src", "*.sln", SearchOption.AllDirectories),
            solution => Run("dotnet", $"build \"{solution}\" --configuration Debug"));

        RunTargets(args);
    }
}
