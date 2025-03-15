using DevTask.Cli.Commands.Abstractions;
using DevTask.Cli.HostedServices;
using DevTask.Cli.Services.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DevTask.Cli.Tests.HostedServicesTests;

public class CommandLineTests
{
    public static IEnumerable<object[]> ExitCommandPermutations = "Ee".ToList()
        .Join("Xx", l => true, r => true, (l, r) => $"{l}{r}")
        .Join("Ii", l => true, r => true, (l, r) => $"{l}{r}")
        .Join("Tt", l => true, r => true, (l, r) => $"{l}{r}")
        .Select(r => new object[] { r });

    private readonly CommandLine _cli;
    private readonly ICliRenderer _mockedRenderer;
    private readonly ICommand _mockedAddTaskCommand;
    private readonly ICommand _mockedDeleteTaskCommand;
    private readonly ICommand _mockedListAllTasksCommand;

    public CommandLineTests()
    {
        _mockedRenderer = Substitute.For<ICliRenderer>();
        _mockedAddTaskCommand = Substitute.For<ICommand>();
        _mockedDeleteTaskCommand = Substitute.For<ICommand>();
        _mockedListAllTasksCommand = Substitute.For<ICommand>();

        _cli = new ServiceCollection()
            .AddScoped(_ => _mockedRenderer)
            .AddKeyedScoped("AddTask", (_, _) => _mockedAddTaskCommand)
            .AddKeyedScoped("DeleteTask", (_, _) => _mockedDeleteTaskCommand)
            .AddKeyedScoped("ListTasks", (_, _) => _mockedListAllTasksCommand)
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
    public void Should_BeRegisteredAsHostedService()
    {
        var services = DevTask.Cli.Program.CreateHostBuilder([])
        .Build()
        .Services;

        var hostedServices = services.GetServices<IHostedService>();

        Assert.Single(hostedServices);
    }

    [Trait("Category", "L0")]
    [Fact]
    public async Task Should_AskTheUserForInputCommand_When_Start()
    {
        _mockedRenderer.AskUserForInputAsync(Arg.Any<CancellationToken>())
            .Returns("exit");

        await _cli.StartAsync(CancellationToken.None);

        await _mockedRenderer.Received(Quantity.Exactly(1))
            .AskUserForInputAsync(Arg.Any<CancellationToken>());
    }

    [Trait("Category", "L0")]
    [Fact]
    public async Task Should_AskTheUserForInputCommand_When_APreviousCommandWasFinishedToBeProcessed()
    {
        _mockedRenderer.AskUserForInputAsync(Arg.Any<CancellationToken>())
            .Returns("some input", "some other input", "exit");

        await _cli.StartAsync(CancellationToken.None);

        await _mockedRenderer.Received(Quantity.Exactly(3))
            .AskUserForInputAsync(Arg.Any<CancellationToken>());
    }

    [Trait("Category", "L0")]
    [Fact]
    public async Task Should_Terminate_When_ExitCommandIsInserted()
    {
        _mockedRenderer.AskUserForInputAsync(Arg.Any<CancellationToken>())
            .Returns("some input", "some other input", "exit", "this should not be reached");

        await _cli.StartAsync(CancellationToken.None);

        await _mockedRenderer.Received(Quantity.Exactly(3))
            .AskUserForInputAsync(Arg.Any<CancellationToken>());
    }

    [Trait("Category", "L0")]
    [Theory]
    [MemberData(nameof(ExitCommandPermutations))]
    [InlineData(" exit ")]
    public async Task Should_BeCaseInsensitive_When_ExitCommandIsInserted(string exitCommand)
    {
        _mockedRenderer.AskUserForInputAsync(Arg.Any<CancellationToken>())
            .Returns("some input", "some other input", exitCommand, "this should not be reached");

        await _cli.StartAsync(CancellationToken.None);

        await _mockedRenderer.Received(Quantity.Exactly(3))
            .AskUserForInputAsync(Arg.Any<CancellationToken>());
    }

    [Trait("Category", "L0")]
    [Fact]
    public async Task Should_InvokeTheAddCommand_When_AddCommandIsInserted()
    {
        _mockedRenderer.AskUserForInputAsync(Arg.Any<CancellationToken>())
            .Returns("add Pass the tests", "exit");

        await _cli.StartAsync(CancellationToken.None);

        await _mockedAddTaskCommand.Received(Quantity.Exactly(1))
            .ExecuteAsync("Pass the tests", Arg.Any<CancellationToken>());
    }

    [Trait("Category", "L0")]
    [Fact]
    public async Task Should_InvokeTheDeleteCommand_When_AddCommandIsInserted()
    {
        var taskIdToDelete = Guid.NewGuid();

        _mockedRenderer.AskUserForInputAsync(Arg.Any<CancellationToken>())
            .Returns($"delete {taskIdToDelete}", "exit");

        await _cli.StartAsync(CancellationToken.None);

        await _mockedDeleteTaskCommand.Received(Quantity.Exactly(1))
            .ExecuteAsync(taskIdToDelete.ToString(), Arg.Any<CancellationToken>());
    }

    [Trait("Category", "L0")]
    [Fact]
    public async Task Should_InvokeTheListCommand_When_AddCommandIsInserted()
    {
        _mockedRenderer.AskUserForInputAsync(Arg.Any<CancellationToken>())
            .Returns("list", "exit");

        await _cli.StartAsync(CancellationToken.None);

        await _mockedListAllTasksCommand.Received(Quantity.Exactly(1))
            .ExecuteAsync(null, Arg.Any<CancellationToken>());
    }

    [Trait("Category", "L0")]
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("skajfjaojfsap")]
    public async Task Should_ShowErrorMessageToUser_When_CommandIsNotRecognized(string? userInput)
    {
        _mockedRenderer.AskUserForInputAsync(Arg.Any<CancellationToken>())
            .Returns(userInput, "exit");

        await _cli.StartAsync(CancellationToken.None);

        await _mockedRenderer.Received(Quantity.Exactly(1))
            .RenderMessageAsync("Command not found", Arg.Any<CancellationToken>());
    }
}