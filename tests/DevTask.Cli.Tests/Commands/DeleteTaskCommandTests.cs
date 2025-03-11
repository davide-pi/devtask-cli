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
public class DeleteTaskCommandTests
{

    [Trait("Category", "L0")]
    [Theory]
    [InlineData(typeof(ICommand))]
    public void Should_InheritFrom(Type typeToInherit)
    {
        typeof(DeleteTaskCommand)
            .Should()
            .BeAssignableTo(typeToInherit);
    }

    [Trait("Category", "L0")]
    [Fact]
    public void Should_DefineItsCommandAs()
    {
        var expectedCommand = "delete";

        var commandField = typeof(DeleteTaskCommand)
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
        var expectedCommandDescription = "Delete an existing task by its ID";

        var descriptionField = typeof(DeleteTaskCommand)
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
    public async Task Should_DeleteTheTask_When_Executed()
    {
        var taskId = Guid.NewGuid();
        var tasksRepositoryMock = new Mock<ITasksRepository>();

        var command = new DeleteTaskCommand(tasksRepositoryMock.Object);

        await command.ExecuteAsync(taskId.ToString(), CancellationToken.None);

        tasksRepositoryMock.Verify(
            r => r.DeleteTaskAsync(taskId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Trait("Category", "L0")]
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task Should_ThrowArgumentException_When_ExecutedWithNullOrEmptyArgument(string? commandArgument)
    {
        var tasksRepositoryMock = new Mock<ITasksRepository>();

        var command = new DeleteTaskCommand(tasksRepositoryMock.Object);

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


    [Trait("Category", "L0")]
    [Fact]
    public async Task Should_ThrowArgumentException_When_ExecutedWithNonGuidArgument()
    {
        var tasksRepositoryMock = new Mock<ITasksRepository>();

        var command = new DeleteTaskCommand(tasksRepositoryMock.Object);

#pragma warning disable CS8604 // Possible null reference argument.
        var action = async () => await command.ExecuteAsync("This surely is not a Guid", CancellationToken.None);
#pragma warning restore CS8604 // Possible null reference argument.

        ( await action
            .Should()
            .ThrowExactlyAsync<ArgumentException>() )
            .And
            .ParamName
            .Should()
            .Be("commandArgument");
    }
}
