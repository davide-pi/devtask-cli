using DevTask.Cli.Commands;
using DevTask.Cli.Commands.Abstractions;
using DevTask.Cli.Repositories.Abstractions;
using FluentAssertions;
using Moq;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DevTask.Cli.Tests.Commands;
public class AddTaskCommandTests
{

    [Trait("Category", "L0")]
    [Theory]
    [InlineData(typeof(ICommand))]
    public void Should_InheritFrom(Type typeToInherit)
    {
        typeof(AddTaskCommand)
            .Should()
            .BeAssignableTo(typeToInherit);
    }

    [Trait("Category", "L0")]
    [Fact]
    public void Should_BeDescribedAs()
    {
        var expectedCommandDescription = "Add a new task to the list";

        var descriptionField = typeof(AddTaskCommand)
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
    public async Task Should_AddANewTask_When_ExecuteMethodIsTriggered()
    {
        var tasksRepositoryMock = new Mock<ITasksRepository>();

        var command = new AddTaskCommand(tasksRepositoryMock.Object);

        await command.ExecuteAsync("test title", CancellationToken.None);

        tasksRepositoryMock.Verify(
            r => r.InsertTaskAsync("test title", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Trait("Category", "L0")]
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task Should_ThrowArgumentException_When_ArgumentIsNullOrEmpty(string? commandArgument)
    {
        var tasksRepositoryMock = new Mock<ITasksRepository>();

        var command = new AddTaskCommand(tasksRepositoryMock.Object);

#pragma warning disable CS8604 // Possible null reference argument.
        var action = async () => await command.ExecuteAsync(commandArgument, CancellationToken.None);
#pragma warning restore CS8604 // Possible null reference argument.

        ( await action
            .Should()
            .ThrowExactlyAsync<ArgumentNullException>() )
            .And
            .ParamName
            .Should()
            .Be("commandArgument");
    }
}
