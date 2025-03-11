using DevTask.Cli.Models;
using DevTask.Cli.Repositories.Abstractions;
using DevTask.Cli.Tests.TestHelpers.Extensions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DevTask.Cli.Tests.Repositories.Abstractions;

public class ITasksRepositoryTests
{
    [Trait("Category", "L0")]
    [Fact]
    public void Should_DefineTheFollowingMethods()
    {
        typeof(ITasksRepository).GetMethods()
            .Should()
            .Satisfy(
                m => m.Name == "InsertTaskAsync"
                     && m.ReturnType == typeof(Task<Guid>)
                     && m.GetParameters().AreOfExpectedTypes(new[] { typeof(string), typeof(CancellationToken) }),
                m => m.Name == "DeleteTaskAsync"
                     && m.ReturnType == typeof(Task)
                     && m.GetParameters().AreOfExpectedTypes(new[] { typeof(Guid), typeof(CancellationToken) }),
                m => m.Name == "GetAllTasksAsync"
                     && m.ReturnType == typeof(Task<IEnumerable<TaskItem>>)
                     && m.GetParameters().AreOfExpectedTypes(new[] { typeof(CancellationToken) })
            );
    }
}
