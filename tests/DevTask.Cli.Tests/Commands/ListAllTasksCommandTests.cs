using DevTask.Cli.Commands;
using DevTask.Cli.Commands.Abstractions;
using DevTask.Cli.Models;
using DevTask.Cli.Repositories.Abstractions;
using DevTask.Cli.Services.Abstractions;
using FluentAssertions;
using Moq;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DevTask.Cli.Tests.Commands;
public class ListAllTasksCommandTests
{

    [Trait("Category", "L0")]
    [Theory]
    [InlineData(typeof(ICommand))]
    public void Should_InheritFrom(Type typeToInherit)
    {
        typeof(ListAllTasksCommand)
            .Should()
            .BeAssignableTo(typeToInherit);
    }

    [Trait("Category", "L0")]
    [Fact]
    public void Should_DefineItsCommandAs()
    {
        var expectedCommand = "list";

        var commandField = typeof(ListAllTasksCommand)
            .GetField("Command", BindingFlags.Public | BindingFlags.Static);

        commandField!.IsInitOnly
            .Should()
            .BeTrue();

        commandField!
            .GetValue(null)
            .Should()
            .BeOfType<string>()
            .And
            .Be(expectedCommand);
    }

    [Trait("Category", "L0")]
    [Fact]
    public void Should_BeDescribedAs()
    {
        var expectedCommandDescription = "List all the tasks";

        var descriptionField = typeof(ListAllTasksCommand)
            .GetField("Description", BindingFlags.Public | BindingFlags.Static);

        descriptionField!.IsInitOnly
            .Should()
            .BeTrue();

        descriptionField!
            .GetValue(null)
            .Should()
            .BeOfType<string>()
            .And
            .Be(expectedCommandDescription);
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
        var tasksRepositoryMock = new Mock<ITasksRepository>();
        tasksRepositoryMock.Setup(r => r.GetAllTasksAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(testTaskList);
        var rendererMock = new Mock<ICliRenderer>();

        var command = new ListAllTasksCommand(tasksRepositoryMock.Object, rendererMock.Object);

        await command.ExecuteAsync(null, CancellationToken.None);

        tasksRepositoryMock.Verify(
            r => r.GetAllTasksAsync(It.IsAny<CancellationToken>()),
            Times.Once);

        rendererMock.Verify(
            c => c.RenderTaskListAsync(testTaskList, CancellationToken.None),
            Times.Once);
    }

    [Trait("Category", "L0")]
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("test")]
    public async Task Should_Not_ThrowArgumentException_When_ExecutedWithNullEmptyOrValorizedArgument(string? commandArgument)
    {
        var tasksRepositoryMock = new Mock<ITasksRepository>();
        var rendererMock = new Mock<ICliRenderer>();

        var command = new ListAllTasksCommand(tasksRepositoryMock.Object, rendererMock.Object);

#pragma warning disable CS8604 // Possible null reference argument.
        var action = async () => await command.ExecuteAsync(commandArgument, CancellationToken.None);
#pragma warning restore CS8604 // Possible null reference argument.

        await action
            .Should()
            .NotThrowAsync();
    }
}
