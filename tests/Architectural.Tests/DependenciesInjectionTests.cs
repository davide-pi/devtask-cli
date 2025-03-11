using DevTask.Cli.Commands;
using DevTask.Cli.Commands.Abstractions;
using DevTask.Cli.Repositories;
using DevTask.Cli.Repositories.Abstractions;
using DevTask.Cli.Services;
using DevTask.Cli.Services.Abstractions;
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

    private static readonly IEnumerable<(Type InerfaceType, Type ImplementationType, ServiceLifetime Lifetime)> _expectedInjectedTypesWithLifetimes = [
        (typeof(ITasksRepository), typeof(JsonFileTasksRepository), ServiceLifetime.Singleton),
        (typeof(ICliRenderer), typeof(CliRenderer), ServiceLifetime.Singleton),
    ];

    private static readonly IEnumerable<(string Key, Type InerfaceType, Type ImplementationType, ServiceLifetime Lifetime)> _expectedInjectedKeyedTypesWithLifetimes = [
        ("AddTask", typeof(ICommand), typeof(AddTaskCommand), ServiceLifetime.Singleton),
        ("DeleteTask", typeof(ICommand), typeof(DeleteTaskCommand), ServiceLifetime.Singleton),
        ("ListTasks", typeof(ICommand), typeof(ListAllTasksCommand), ServiceLifetime.Singleton),
    ];

    public static readonly IEnumerable<object[]> ExpectedInjectedTypesWithLifetimes = _expectedInjectedTypesWithLifetimes
        .Select<(Type InerfaceType, Type ImplementationType, ServiceLifetime Lifetime), object[]>(t => [t.InerfaceType, t.ImplementationType, t.Lifetime]);
    
    public static readonly IEnumerable<object[]> ExpectedInjectedKeyedTypesWithLifetimes = _expectedInjectedKeyedTypesWithLifetimes
        .Select<(string Key, Type InerfaceType, Type ImplementationType, ServiceLifetime Lifetime), object[]>(t => [t.Key, t.InerfaceType, t.ImplementationType, t.Lifetime]);

    private static readonly IServiceProvider _services = DevTask.Cli.Program.CreateHostBuilder([])
        .Build()
        .Services;

    [Trait("Category", "L0")]
    [Fact]
    public void AnyClassOrInterface_Should_Not_BeForgotenInTheInjection()
    {
        var expectedInjectedTypes = _expectedInjectedTypesWithLifetimes
            .Select(e => e.InerfaceType);

        var expectedInjectedKeyedTypes = _expectedInjectedKeyedTypesWithLifetimes
            .Select(e => e.InerfaceType);

        var forgottenTypes = _assembly.GetTypes()
            .Where(t => t.IsClass || t.IsInterface)
            .Where(t => t.GetCustomAttribute<CompilerGeneratedAttribute>() == null)
            .Where(t => !_nonInjectedNamespaces.Any(n => t.Namespace!.StartsWith(n)))
            .Where(t => !_nonInjectedTypes.Contains(t))
            .Where(t => !expectedInjectedTypes.Contains(t))
            .Where(t => !expectedInjectedKeyedTypes.Contains(t))
            .Where(t => !t.GetInterfaces().Any(i => _nonInjectedTypes.Contains(i)))
            .Where(t => !t.GetInterfaces().Any(i => expectedInjectedTypes.Contains(i)))
            .Where(t => !t.GetInterfaces().Any(i => expectedInjectedKeyedTypes.Contains(i)));

        forgottenTypes
            .Should()
            .BeEmpty();
    }

    [Trait("Category", "L0")]
    [Theory]
    [MemberData(nameof(ExpectedInjectedTypesWithLifetimes))]
    public void ServiceProvider_Should_ContainsTheServiceWithExpectedLifetime(Type expectedInjectedType, Type expectedImplementationType, ServiceLifetime lifetime)
    {
        var scope = _services.CreateScope();
        var resolved = scope.ServiceProvider.GetService(expectedInjectedType);

        resolved
            .Should()
            .NotBeNull()
            .And
            .BeAssignableTo(expectedImplementationType);

        var resolved2 = scope.ServiceProvider.GetService(expectedInjectedType);

        var newScope = _services.CreateScope();
        var resolvedNewScope = newScope.ServiceProvider.GetService(expectedInjectedType);
        AssertLifetime(lifetime, resolved, resolved2, resolvedNewScope);
    }


    [Trait("Category", "L0")]
    [Theory]
    [MemberData(nameof(ExpectedInjectedKeyedTypesWithLifetimes))]
    public void ServiceProvider_Should_ContainsTheKeyedServiceWithExpectedLifetime(string expectedKey, Type expectedInjectedType, Type expectedImplementationType, ServiceLifetime lifetime)
    {
        var scope = _services.CreateScope();
        var resolved = scope.ServiceProvider.GetRequiredKeyedService(expectedInjectedType, expectedKey);

        resolved
            .Should()
            .NotBeNull()
            .And
            .BeAssignableTo(expectedImplementationType);

        var resolved2 = scope.ServiceProvider.GetRequiredKeyedService(expectedInjectedType, expectedKey);

        var newScope = _services.CreateScope();
        var resolvedNewScope = newScope.ServiceProvider.GetRequiredKeyedService(expectedInjectedType, expectedKey);
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