using DevTask.Cli.Commands;
using DevTask.Cli.Commands.Abstractions;
using DevTask.Cli.Models;
using DevTask.Cli.Repositories.Abstractions;
using DevTask.Cli.Services.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DevTask.Cli.Tests.Commands;
public class ListAllTasksCommandTests
{
    private readonly ITasksRepository _mockedTasksRepository;
    private readonly ICliRenderer _mockedRenderer;
    private readonly ListAllTasksCommand _command;

    public ListAllTasksCommandTests()
    {
        _mockedTasksRepository = Substitute.For<ITasksRepository>();
        _mockedRenderer = Substitute.For<ICliRenderer>();

        _command = new ServiceCollection()
            .AddScoped(_ => _mockedTasksRepository)
            .AddScoped(_ => _mockedRenderer)
            .AddScoped<ListAllTasksCommand>()
            .BuildServiceProvider()
            .GetRequiredService<ListAllTasksCommand>();
    }

    [Trait("Category", "L0")]
    [Fact]
    public void Should_InheritFromICommand()
    {
        Assert.IsAssignableFrom<ICommand>(_command);
    }

    [Trait("Category", "L0")]
    [Fact]
    public void Should_DefineItsCommandAs()
    {
        var expectedCommand = "list";

        var commandField = _command.GetType()
            .GetField("Command", BindingFlags.Public | BindingFlags.Static);

        Assert.True(commandField!.IsInitOnly, "Field should be init only");
        Assert.IsType<string>(commandField.GetValue(null));
        Assert.Equal(expectedCommand, commandField.GetValue(null));
    }

    [Trait("Category", "L0")]
    [Fact]
    public void Should_BeDescribedAs()
    {
        var descriptionField = _command.GetType()
            .GetField("Description", BindingFlags.Public | BindingFlags.Static);

        Assert.True(descriptionField!.IsInitOnly, "Field should be init only");

        var descriptionValue = descriptionField!.GetValue(null);

        Assert.IsAssignableFrom<string>(descriptionValue);
        Assert.Equal("List all the tasks", descriptionValue);
    }

    [Trait("Category", "L0")]
    [Fact]
    public async Task Should_ShowAllTheTasks_When_Executed()
    {
        var testTaskList = new List<TaskItem>()
        {
                new(Guid.NewGuid(), "Test task 1"),
                new(Guid.NewGuid(), "Test task 2"),
                new(Guid.NewGuid(), "Test task 3")
        };

        _mockedTasksRepository.GetAllTasksAsync(Arg.Any<CancellationToken>())
            .Returns(testTaskList);

        await _command.ExecuteAsync(null, CancellationToken.None);

        await _mockedTasksRepository.Received(Quantity.Exactly(1))
            .GetAllTasksAsync(Arg.Any<CancellationToken>());

        await _mockedRenderer.Received(Quantity.Exactly(1))
            .RenderTaskListAsync(testTaskList, CancellationToken.None);
    }

    [Trait("Category", "L0")]
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("test")]
    public async Task Should_Not_ThrowArgumentException_When_ExecutedWithNullEmptyOrValuedArgument(string? commandArgument)
    {
#pragma warning disable CS8604 // Possible null reference argument.
        var action = async () => await _command.ExecuteAsync(commandArgument, CancellationToken.None);
#pragma warning restore CS8604 // Possible null reference argument.

        var exceptions = await Record.ExceptionAsync(action);
        Assert.Null(exceptions);
    }
}
