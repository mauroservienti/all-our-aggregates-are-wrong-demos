using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using static Bullseye.Targets;
using static SimpleExec.Command;
using Console = Colorful.Console;

internal class Program
{
    static readonly string customSdkInstallDir = ".dotnet";
    static readonly string buildSupportDir = ".build";
    public static void Main(string[] args)
    {
        Target("default", DependsOn("verify-OS-is-suppported"),
            Directory.EnumerateFiles("src", "*.sln", SearchOption.AllDirectories),
            solution =>
            {
                var (customSdk, sdkPath, sdkVersion) = EnsureRequiredSdkIsInstalled();
                Console.WriteLine($"Build will be executed using {(customSdk ? "user defined SDK" : "default SDK")}, Version '{sdkVersion}'.{(customSdk ? $" Installed at '{sdkPath}'" : "")}");
                Run($"{Path.Combine(sdkPath, "dotnet")}", $"build \"{solution}\" --configuration Debug");
            });

        Target("verify-OS-is-suppported",
            () =>
            {
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    throw new InvalidOperationException("Build is supported on Windows only, at this time.");
                }
            });

        RunTargets(args);
    }

    static (bool customSdk, string sdkPath, string sdkVersion) EnsureRequiredSdkIsInstalled()
    {
        var currentSdkVersion = Read("dotnet", "--version").TrimEnd(Environment.NewLine.ToCharArray());
        var requiredSdkFile = Directory.EnumerateFiles(".", ".required-sdk", SearchOption.TopDirectoryOnly).SingleOrDefault();

        if (string.IsNullOrWhiteSpace(requiredSdkFile))
        {
            Console.WriteLine("No custom SDK is required.", Color.Green);
            return (false, "", currentSdkVersion);
        }

        var requiredSdkVersion = File.ReadAllText(requiredSdkFile).TrimEnd(Environment.NewLine.ToCharArray());

        if (string.Compare(currentSdkVersion, requiredSdkVersion) == 0)
        {
            Console.WriteLine("Insalled SDK is the same as required one, '.required-sdk' file is not necessary. Build will use the SDK available on the machine.", Color.Yellow);
            return (false, "", currentSdkVersion);
        }

        Console.WriteLine($"Installed SDK ({currentSdkVersion}) doesn't match required one ({requiredSdkVersion}).", Color.Yellow);
        Console.WriteLine($"{requiredSdkVersion} will be installed and and used to run the build.", Color.Yellow);

        var installScriptName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? "dotnet-install.ps1"
            : "dotnet-install.sh";

        var installScriptUrl = $"https://dot.net/v1/{installScriptName}";

        Console.WriteLine($"Downloading {installScriptName} script, from {installScriptUrl}.");

        Directory.CreateDirectory(buildSupportDir);
        new WebClient().DownloadFile(installScriptUrl, Path.Combine(buildSupportDir, installScriptName));

        Console.WriteLine($"Ready to install custom SDK, version {requiredSdkVersion}.");

        var installScriptLocation = Path.Combine(".", buildSupportDir, installScriptName);
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Run("powershell", $@"{installScriptLocation} -Version {requiredSdkVersion} -InstallDir {customSdkInstallDir}");
        }
        else
        {
            Run("bash", $@"{installScriptLocation} --version {requiredSdkVersion} --install-dir {customSdkInstallDir}");
        }

        return (true, customSdkInstallDir, requiredSdkVersion);
    }
}