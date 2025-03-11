using DevTask.Cli.Commands.Abstractions;
using DevTask.Cli.Tests.TestHelpers.Extensions;
using FluentAssertions;
using System.Threading;
using System.Threading.Tasks;

namespace DevTask.Cli.Tests.Commands.Abstractions;

public class ICommandTests
{
    [Trait("Category", "L0")]
    [Fact]
    public void Should_DefineTheFollowingMethods()
    {
        typeof(ICommand).GetMethods()
            .Should()
            .Satisfy(
                m => m.Name == nameof(ICommand.ExecuteAsync)
                     && m.ReturnType == typeof(Task)
                     && m.GetParameters().AreOfExpectedTypes(new[] { typeof(string), typeof(CancellationToken) })
            );
    }
}
