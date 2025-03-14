using DevTask.Cli.Commands;
using DevTask.Cli.Commands.Abstractions;
using DevTask.Cli.Repositories.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DevTask.Cli.Tests.Commands;
public class DeleteTaskCommandTests
{
    private readonly ITasksRepository _mockedTasksRepository;
    private readonly DeleteTaskCommand _command;

    public DeleteTaskCommandTests()
    {
        _mockedTasksRepository = Substitute.For<ITasksRepository>();

        _command = new ServiceCollection()
            .AddScoped(_ => _mockedTasksRepository)
            .AddScoped<DeleteTaskCommand>()
            .BuildServiceProvider()
            .GetRequiredService<DeleteTaskCommand>();
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
        var expectedCommand = "delete";

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
        Assert.Equal("Delete an existing task by its ID", descriptionValue);
    }

    [Trait("Category", "L0")]
    [Fact]
    public async Task Should_DeleteTheTask_When_Executed()
    {
        var taskId = Guid.NewGuid();

        await _command.ExecuteAsync(taskId.ToString(), CancellationToken.None);

        await _mockedTasksRepository.Received(Quantity.Exactly(1))
            .DeleteTaskAsync(taskId, Arg.Any<CancellationToken>());
    }

    [Trait("Category", "L0")]
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task Should_ThrowArgumentException_When_ExecutedWithNullOrEmptyArgument(string? commandArgument)
    {
#pragma warning disable CS8604 // Possible null reference argument.
        var action = async () => await _command.ExecuteAsync(commandArgument, CancellationToken.None);
#pragma warning restore CS8604 // Possible null reference argument.

        await Assert.ThrowsAsync<ArgumentNullException>("commandArgument", action);
    }


    [Trait("Category", "L0")]
    [Fact]
    public async Task Should_ThrowArgumentException_When_ExecutedWithNonGuidArgument()
    {
#pragma warning disable CS8604 // Possible null reference argument.
        var action = async () => await _command.ExecuteAsync("This surely is not a Guid", CancellationToken.None);
#pragma warning restore CS8604 // Possible null reference argument.

        await Assert.ThrowsAsync<ArgumentException>(action);
    }
}
