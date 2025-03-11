using DevTask.Cli.Models;
using DevTask.Cli.Tests.TestHelpers.Extensions;
using FluentAssertions;
using System;
using System.Reflection;

namespace DevTask.Cli.Tests.Models;

public class TaskItemTests
{
    [Trait("Category", "L0")]
    [Fact]
    public void Should_DefineTheFollowingProperties()
    {
        typeof(TaskItem).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Should()
            .Satisfy(
                static p => p.Name == nameof(TaskItem.Id) && p.PropertyType == typeof(Guid) && p.IsInitOnly(),
                static p => p.Name == nameof(TaskItem.Title) && p.PropertyType == typeof(string) && p.IsInitOnly()
            );
    }

    [Trait("Category", "L0")]
    [Fact]
    public void Should_DefineTheFollowingPublicConstrutors()
    {
        typeof(TaskItem).GetConstructors(BindingFlags.Public | BindingFlags.Instance)
            .Should()
            .Satisfy(
                static p => p.GetParameters().AreOfExpectedTypes(new[] { typeof(Guid), typeof(string) })
            );
    }

    [Trait("Category", "L0")]
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Should_ThrowAnException_When_ConstructorIscalledWithNullOrEmptyTitle(string? title)
    {
#pragma warning disable CS8604 // Possible null reference argument.
        var action = () => new TaskItem(Guid.NewGuid(), title);
#pragma warning restore CS8604 // Possible null reference argument.

        action
            .Should()
            .ThrowExactly<ArgumentNullException>()
            .And
            .ParamName
            .Should()
            .Be("title");
    }
}
