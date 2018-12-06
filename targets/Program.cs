using System;
using System.Drawing;
using System.IO;
using System.Linq;
using static Bullseye.Targets;
using static SimpleExec.Command;
using Console = Colorful.Console;

internal class Program
{
    static string customSdkInstallDir = ".dotnet";
    public static void Main(string[] args)
    {
        Target("default",
            Directory.EnumerateFiles("src", "*.sln", SearchOption.AllDirectories),
            solution =>
            {
                var (customSdk, sdkPath, sdkVersion) = EnsureRequiredSdkIsInstalled();
                Console.WriteLine($"Build will be executed using {(customSdk ? "user defined SDK" : "default SDK")}, Version '{sdkVersion}'.{(customSdk ? $" Installed at '{sdkPath}'" : "")}");
                Run($"{sdkPath}dotnet", $"build \"{solution}\" --configuration Debug");
            });

        RunTargets(args);
    }

    static (bool customSdk, string sdkPath, string sdkVersion) EnsureRequiredSdkIsInstalled()
    {
        var currentSdkVersion = Read("dotnet", "--version").TrimEnd(Environment.NewLine.ToCharArray());
        var requiredSdkFile = Directory.EnumerateFiles(".", ".required-sdk", SearchOption.TopDirectoryOnly).SingleOrDefault();

        if (string.IsNullOrWhiteSpace(requiredSdkFile))
        {
            return (false, "", currentSdkVersion);
        }

        var requiredSdkVersion = File.ReadAllText(requiredSdkFile).TrimEnd(Environment.NewLine.ToCharArray());

        switch (string.Compare(currentSdkVersion, requiredSdkVersion))
        {
            case 0: //equals
                throw new InvalidOperationException("Insalled SDK is the same as required one, '.required-sdk' file is not necessary.");

            case 1: //current is greater
                Console.WriteLine($"Installed SDK ({currentSdkVersion}) is more recent than required one ({requiredSdkVersion}).", Color.Yellow);
                Console.WriteLine($"{requiredSdkVersion} will be installed and and used to run the build", Color.Yellow);
                break;
            case -1: //required is greater
                Console.WriteLine($"Required SDK ({requiredSdkVersion}) is more recent than installed one ({currentSdkVersion}).", Color.Green);
                Console.WriteLine($"{requiredSdkVersion} will be installed and and used to run the build", Color.Green);
                break;
        }

        Run("powershell", $@".\targets\dotnet-install.ps1 -Version {requiredSdkVersion} -InstallDir {customSdkInstallDir}");

        return (true, $@"{customSdkInstallDir}\", requiredSdkVersion);
    }
}