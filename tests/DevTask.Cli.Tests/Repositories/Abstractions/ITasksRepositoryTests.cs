using DevTask.Cli.Models;
using DevTask.Cli.Repositories.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DevTask.Cli.Tests.Repositories.Abstractions;

public class ITasksRepositoryTests
{
    public static IEnumerable<object[]> ExpectedMethods = [
        [nameof(ITasksRepository.InsertTaskAsync), typeof(Task<Guid>), new Type[] { typeof(string), typeof(CancellationToken) }],
        [nameof(ITasksRepository.DeleteTaskAsync), typeof(Task), new Type[] { typeof(Guid), typeof(CancellationToken) }],
        [nameof(ITasksRepository.GetAllTasksAsync), typeof(Task<IEnumerable<TaskItem>>), new Type[] { typeof(CancellationToken) }]
    ];

    [Trait("Category", "L0")]
    [Fact]
    public void Should_DefineExpectedNumberOfMethods()
    {
        var properties = typeof(ITasksRepository).GetMethods(BindingFlags.Public | BindingFlags.Instance);

        Assert.Equal(ExpectedMethods.Count(), properties.Length);
    }

    [Trait("Category", "L0")]
    [Theory]
    [MemberData(nameof(ExpectedMethods))]
    public void Should_DefineTheMethodAs(string methodName, Type expectedReturnType, Type[] parameters)
    {
        var method = typeof(ITasksRepository).GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance, parameters)!;

        Assert.NotNull(method);
        Assert.Equal(expectedReturnType, method.ReturnType);
    }
}
