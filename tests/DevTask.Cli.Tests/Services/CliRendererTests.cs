using DevTask.Cli.Repositories.Abstractions;
using DevTask.Cli.Repositories;
using System;
using DevTask.Cli.Services.Abstractions;
using FluentAssertions;
using DevTask.Cli.Services;
using DevTask.Cli.Models;
using System.Collections.Generic;
using Spectre.Console;
using Moq;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;

namespace DevTask.Cli.Tests.Services;
public class CliRendererTests
{
    [Trait("Category", "L0")]
    [Theory]
    [InlineData(typeof(ICliRenderer))]
    public void Should_InheritFrom(Type typeToInherit)
    {
        typeof(CliRenderer)
            .Should()
            .BeAssignableTo(typeToInherit);
    }

    [Trait("Category", "L0")]
    [Fact]
    public async Task Should_DrawATableWithAllTasks_When_RenderTaskListIsExecutedAsync()
    {
        var testTaskList = new List<TaskItem>()
        {
                new(Guid.NewGuid(), "Test task 1"),
                new(Guid.NewGuid(), "Test task 2"),
                new(Guid.NewGuid(), "Test task 3")
        };

        var consoleMock = new Mock<IAnsiConsole>();

        var renderer = new CliRenderer(consoleMock.Object);

        await renderer.RenderTaskListAsync(testTaskList, CancellationToken.None);

        consoleMock.Verify(
            c => c.Write(It.IsAny<Table>()));

        consoleMock.Verify(
            c => c.Write(It.Is<Table>(t => t.Columns.Count == 2 && t.Rows.Count == testTaskList.Count)),
            Times.Once);
    }

}
