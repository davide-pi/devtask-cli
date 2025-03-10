using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DevTask.Cli.Tests.ArchitecturalTests;

public class DependenciesInjectionTests
{
    private static readonly Assembly _assembly = Assembly.Load("DevTask.Cli");

    private static readonly List<Type> _nonInjectedTypes =
    [
        typeof(DevTask.Cli.Program),
        typeof(DevTask.Cli.HostedServices.CommandLine)
    ];
    private static readonly List<string> _nonInjectedNamespaces =
    [
        "DevTask.Cli.Models"
    ];

    public static readonly IEnumerable<(Type Type, ServiceLifetime Lifetime)> _expectedInjectedTypesWithLifetimes = [
        //(typeof(IInterface), ServiceLifetime.Singleton),
    ];

    public static readonly IEnumerable<object[]> ExpectedInjectedTypesWithLifetimes = _expectedInjectedTypesWithLifetimes
        .Select<(Type Type, ServiceLifetime Lifetime), object[]>(t => [t.Type, t.Lifetime]);

    private static readonly IServiceProvider _services = DevTask.Cli.Program.CreateHostBuilder([])
        .Build()
        .Services;

    [Trait("Category", "L0")]
    [Fact]
    public void AnyClassOrInterface_Should_Not_BeForgotenInTheInjection()
    {
        var expectedInjectedTypes = _expectedInjectedTypesWithLifetimes
            .Select(e => e.Type);

        var forgottenTypes = _assembly.GetTypes()
            .Where(t => t.IsClass || t.IsInterface)
            .Where(t => t.GetCustomAttribute<CompilerGeneratedAttribute>() == null)
            .Where(t => !_nonInjectedNamespaces.Any(n => t.Namespace!.StartsWith(n)))
            .Where(t => !_nonInjectedTypes.Contains(t))
            .Where(t => !expectedInjectedTypes.Contains(t))
            .Where(t => !t.GetInterfaces().Any(i => _nonInjectedTypes.Contains(i)))
            .Where(t => !t.GetInterfaces().Any(i => expectedInjectedTypes.Contains(i)));

        forgottenTypes
            .Should()
            .BeEmpty();
    }

    [Trait("Category", "L0")]
    [Theory]
    [MemberData(nameof(ExpectedInjectedTypesWithLifetimes))]
    public void ServiceProvider_Should_ContainsTheServiceWithExpectedLifetime(Type expectedInjectedType, ServiceLifetime lifetime)
    {
        var scope = _services.CreateScope();
        var resolved = scope.ServiceProvider.GetService(expectedInjectedType);

        resolved
            .Should()
            .NotBeNull();

        var resolved2 = scope.ServiceProvider.GetService(expectedInjectedType);

        var scope2 = _services.CreateScope();
        var resolvedNewScope = scope2.ServiceProvider.GetService(expectedInjectedType);
        AssertLifetime(lifetime, resolved, resolved2, resolvedNewScope);
    }

    private static void AssertLifetime(ServiceLifetime lifetime, object resolved, object? resolved2, object? resolvedNewScope)
    {
        switch (lifetime)
        {
            case ServiceLifetime.Singleton:
                resolved
                    .Should()
                    .Be(resolved2)
                    .And
                    .Be(resolvedNewScope);
                break;
            case ServiceLifetime.Scoped:
                resolved
                    .Should()
                    .Be(resolved2)
                    .And
                    .NotBe(resolvedNewScope);
                break;

            case ServiceLifetime.Transient:
                resolved
                    .Should()
                    .NotBe(resolved2)
                    .And
                    .NotBe(resolvedNewScope);
                break;
            default:
                throw new NotImplementedException($"Lifetime {lifetime} is not supported yet");
        }
    }
}