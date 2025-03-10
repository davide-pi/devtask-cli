using DevTask.Cli.HostedServices;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Reflection;

namespace DevTask.Cli.Tests.HostedServicesTests;

public class CommandLineTests
{
    private static readonly Assembly _assembly = Assembly.Load("DevTask.Cli");

    [Trait("Category", "L0")]
    [Theory]
    [InlineData(typeof(IHostedService))]
    public void Should_InheritFrom(Type typeToInherit)
    {
        typeof(CommandLine)
            .Should()
            .BeAssignableTo(typeToInherit);
    }

    [Trait("Category", "L0")]
    [Fact]
    public void CommandLine_Should_BeRegisteredAsHostedService()
    {
        var services = DevTask.Cli.Program.CreateHostBuilder([])
        .Build()
        .Services;

        var hostedServices = services.GetServices<IHostedService>();

        hostedServices.SingleOrDefault(s => s.GetType() == typeof(CommandLine))
            .Should()
            .NotBeNull();
    }
}