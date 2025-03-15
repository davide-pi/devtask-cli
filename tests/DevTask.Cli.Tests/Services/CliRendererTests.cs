using DevTask.Cli.Models;
using DevTask.Cli.Services;
using DevTask.Cli.Services.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using Spectre.Console;
using Spectre.Console.Testing;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DevTask.Cli.Tests.Services;
public class CliRendererTests
{
    private readonly IAnsiConsole _mockedConsole;
    private readonly CliRenderer _renderer;

    public CliRendererTests()
    {
        _mockedConsole = Substitute.For<IAnsiConsole>();

        _renderer = new ServiceCollection()
            .AddScoped(_ => _mockedConsole)
            .AddScoped<CliRenderer>()
            .BuildServiceProvider()
            .GetRequiredService<CliRenderer>();
    }

    [Trait("Category", "L0")]
    [Fact]
    public void Should_InheritFromICommand()
    {
        Assert.IsAssignableFrom<ICliRenderer>(_renderer);
    }

    [Trait("Category", "L0")]
    [Fact]
    public async Task Should_DrawATableWithAllTasks_When_RenderTaskListIsExecuted()
    {
        var testTaskList = new List<TaskItem>()
        {
                new(Guid.NewGuid(), "Test task 1"),
                new(Guid.NewGuid(), "Test task 2"),
                new(Guid.NewGuid(), "Test task 3")
        };

        await _renderer.RenderTaskListAsync(testTaskList, CancellationToken.None);

        _mockedConsole.Received(Quantity.Exactly(1))
            .Write(Arg.Any<Table>());

        _mockedConsole.Received(Quantity.Exactly(1))
            .Write(Arg.Is<Table>(t => t.Columns.Count == 2 && t.Rows.Count == testTaskList.Count));
    }

    [Trait("Category", "L0")]
    [Fact]
    public async Task Should_WaitForTheUserInput_When_AskUserForInputIsExecuted()
    {
        await _renderer.AskUserForInputAsync(CancellationToken.None);

        _mockedConsole.Received(Quantity.Exactly(1))
            .Ask<string>(">:");
    }

    [Trait("Category", "L0")]
    [Fact]
    public async Task Should_ReturnTheUserInput_When_AskUserForInputIsExecuted()
    {
        using var fakeConsole = new TestConsole();
        fakeConsole.Input.PushTextWithEnter("Some text inserted from the user");

        var renderer = new CliRenderer(fakeConsole);

        var userInput = await renderer.AskUserForInputAsync(CancellationToken.None);

        Assert.Equal("Some text inserted from the user", userInput);
    }


    [Trait("Category", "L0")]
    [Fact]
    public async Task Should_RenderMessageForUser_When_RenderMessageIsExecuted()
    {
        var fakeConsole = new TestConsole();

        var renderer = new CliRenderer(fakeConsole);

        await renderer.RenderMessageAsync("My message for the user", CancellationToken.None);

        Assert.Matches("My message for the user(?:\r\n|\n)", fakeConsole.Output);
    }
}
