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
public class AddTaskCommandTests
{
    private readonly ITasksRepository _mockedTasksRepository;
    private readonly AddTaskCommand _command;

    public AddTaskCommandTests()
    {
        _mockedTasksRepository = Substitute.For<ITasksRepository>();

        _command = new ServiceCollection()
            .AddScoped(_ => _mockedTasksRepository)
            .AddScoped<AddTaskCommand>()
            .BuildServiceProvider()
            .GetRequiredService<AddTaskCommand>();
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
        var expectedCommand = "add";

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
        Assert.Equal("Add a new task", descriptionValue);
    }

    [Trait("Category", "L0")]
    [Fact]
    public async Task Should_AddANewTask_When_Executed()
    {
        await _command.ExecuteAsync("test title", CancellationToken.None);

        await _mockedTasksRepository.Received(Quantity.Exactly(1))
            .InsertTaskAsync("test title", Arg.Any<CancellationToken>());
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
}
