using DevTask.Cli.Models;
using DevTask.Cli.Tests.TestHelpers.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DevTask.Cli.Tests.Models;

public class TaskItemTests
{
    public static IEnumerable<object[]> ExpectedProperties = [
        [nameof(TaskItem.Id), typeof(Guid), true],
        [nameof(TaskItem.Title), typeof(string), true]
    ];
    public static IEnumerable<object[]> ExpectedConstructors = [
        [new Type[] {typeof(Guid), typeof(string) }]
    ];

    [Trait("Category", "L0")]
    [Fact]
    public void Should_DefineExpectedNumberOfProperties()
    {
        var properties = typeof(TaskItem).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        Assert.Equal(ExpectedProperties.Count(), properties.Length);
    }

    [Trait("Category", "L0")]
    [Theory]
    [MemberData(nameof(ExpectedProperties))]
    public void Should_DefineThePropertyAs(string propertyName, Type expectedPropertyType, bool shouldBeInitOnly)
    {
        var property = typeof(TaskItem).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance)!;

        Assert.Multiple([
            () => Assert.Equal(expectedPropertyType, property.PropertyType),
            () => Assert.Equal(shouldBeInitOnly, property.IsInitOnly()),
        ]);
    }

    [Trait("Category", "L0")]
    [Fact]
    public void Should_DefineExpectedNumberOfConstructors()
    {
        var constructors = typeof(TaskItem).GetConstructors(BindingFlags.Public | BindingFlags.Instance);

        Assert.Equal(ExpectedConstructors.Count(), constructors.Length);
    }

    [Trait("Category", "L0")]
    [Theory]
    [MemberData(nameof(ExpectedConstructors))]
    public void Should_DefineTheConstructorAS(Type[] parameters)
    {
        var constructor = typeof(TaskItem).GetConstructor(BindingFlags.Public | BindingFlags.Instance, parameters);

        Assert.NotNull(constructor);
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

        Assert.Throws<ArgumentNullException>("title", action);
    }
}
