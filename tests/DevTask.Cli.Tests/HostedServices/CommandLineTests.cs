using DevTask.Cli.HostedServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DevTask.Cli.Tests.HostedServicesTests;

public class CommandLineTests
{
    private readonly CommandLine _cli;

    public CommandLineTests()
    {
        _cli = new ServiceCollection()
            .AddScoped<CommandLine>()
            .BuildServiceProvider()
            .GetRequiredService<CommandLine>();
    }

    [Trait("Category", "L0")]
    [Fact]
    public void Should_InheritFromICommand()
    {
        Assert.IsAssignableFrom<IHostedService>(_cli);
    }

    [Trait("Category", "L0")]
    [Fact]
    public void CommandLine_Should_BeRegisteredAsHostedService()
    {
        var services = DevTask.Cli.Program.CreateHostBuilder([])
        .Build()
        .Services;

        var hostedServices = services.GetServices<IHostedService>();

        Assert.Single(hostedServices);
    }
}