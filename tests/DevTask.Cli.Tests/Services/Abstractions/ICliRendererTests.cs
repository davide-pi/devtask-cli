using DevTask.Cli.Models;
using DevTask.Cli.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DevTask.Cli.Tests.Services.Abstractions;

public class ICliRendererTests
{
    public static IEnumerable<object[]> ExpectedMethods = [
        [nameof(ICliRenderer.RenderTaskListAsync), typeof(Task), new Type[] { typeof(IEnumerable<TaskItem>), typeof(CancellationToken) }],
        [nameof(ICliRenderer.AskUserForInputAsync), typeof(Task<string>), new Type[] { typeof(CancellationToken) }],
        [nameof(ICliRenderer.RenderMessageAsync), typeof(Task), new Type[] { typeof(string), typeof(CancellationToken) }]
    ];

    [Trait("Category", "L0")]
    [Fact]
    public void Should_DefineExpectedNumberOfMethods()
    {
        var properties = typeof(ICliRenderer).GetMethods(BindingFlags.Public | BindingFlags.Instance);

        Assert.Equal(ExpectedMethods.Count(), properties.Length);
    }

    [Trait("Category", "L0")]
    [Theory]
    [MemberData(nameof(ExpectedMethods))]
    public void Should_DefineTheMethodAs(string methodName, Type expectedReturnType, Type[] parameters)
    {
        var method = typeof(ICliRenderer).GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance, parameters)!;

        Assert.NotNull(method);
        Assert.Equal(expectedReturnType, method.ReturnType);
    }
}
