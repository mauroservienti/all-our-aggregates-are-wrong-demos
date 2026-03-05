using NServiceBus.IntegrationTesting;
using Xunit.Abstractions;

namespace EndToEndTests;

public class ATest(ITestOutputHelper testOutput, ATestDependencies dependencies) : IClassFixture<ATestDependencies>, IAsyncDisposable
{
    readonly TestEnvironment environment = dependencies.Environment ?? throw new NullReferenceException(nameof(dependencies.Environment));
    bool _testPassed;

    [Fact]
    public async Task ATest_that_tests_something()
    {
        var webApp = environment!.GetEndpoint("WebApp");
        var webAppBaseUrl = webApp.GetBaseUrl(5030);
        var client = new HttpClient()
        {
            BaseAddress = new Uri(webAppBaseUrl)
        };

        var response = await client.GetStringAsync("/");

        _testPassed = true;
    }

    public async ValueTask DisposeAsync()
    {
        await environment.DropAndRecreateDatabases();
    }
}