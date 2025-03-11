using DevTask.Cli.Models;
using DevTask.Cli.Services.Abstractions;
using DevTask.Cli.Tests.TestHelpers.Extensions;
using FluentAssertions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DevTask.Cli.Tests.Services.Abstractions;

public class ICliRendererTests
{
    [Trait("Category", "L0")]
    [Fact]
    public void Should_DefineTheFollowingMethods()
    {
        typeof(ICliRenderer).GetMethods()
            .Should()
            .Satisfy(
                m => m.Name == nameof(ICliRenderer.RenderTaskListAsync)
                     && m.ReturnType == typeof(Task)
                     && m.GetParameters().AreOfExpectedTypes(new[] { typeof(IEnumerable<TaskItem>), typeof(CancellationToken) })
            );
    }
}
