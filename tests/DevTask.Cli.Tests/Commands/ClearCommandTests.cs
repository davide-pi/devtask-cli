using DevTask.Cli.Commands;
using DevTask.Cli.Commands.Abstractions;
using DevTask.Cli.Services.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DevTask.Cli.Tests.Commands;
public class ClearCommandTests
{
    private readonly ICliRenderer _mockedRenderer;
    private readonly ClearCommand _command;

    public ClearCommandTests()
    {
        _mockedRenderer = Substitute.For<ICliRenderer>();

        _command = new ServiceCollection()
            .AddScoped(_ => _mockedRenderer)
            .AddScoped<ClearCommand>()
            .BuildServiceProvider()
            .GetRequiredService<ClearCommand>();
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
        var expectedCommand = "clear";

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
        Assert.Equal("Clean the CLI", descriptionValue);
    }

    [Trait("Category", "L0")]
    [Fact]
    public async Task Should_CleanTheCli_When_Executed()
    {
        await _command.ExecuteAsync(null, CancellationToken.None);

        await _mockedRenderer.Received(Quantity.Exactly(1))
            .ClearAsync(Arg.Any<CancellationToken>());
    }
}
