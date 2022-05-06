using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using static Bullseye.Targets;
using static SimpleExec.Command;

internal class Program
{
    public static async Task Main(string[] args)
    {
        var sdk = new DotnetSdkManager();
        var dotnet = await sdk.GetDotnetCliPath();

        Target("default", DependsOn("verify-OS-is-suppported"),
            Directory.EnumerateFiles("src", "*.sln", SearchOption.AllDirectories),
            solution => Run(dotnet, $"build \"{solution}\" --configuration Debug"));

        Target(
            "verify-OS-is-suppported",
            () => { if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) throw new InvalidOperationException("Build is supported on Windows only, at this time."); });

        RunTargetsAndExit(args);
    }
}
