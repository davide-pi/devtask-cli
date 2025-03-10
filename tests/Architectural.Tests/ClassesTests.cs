using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DevTask.Cli.Tests.ArchitecturalTests;

public class ClassesTests
{
    private static readonly Assembly _assembly = Assembly.Load("DevTask.Cli");

    private static readonly List<Type> UnsealedClasses =
    [
        typeof(DevTask.Cli.Program)
    ];

    public static readonly IEnumerable<object[]> ExpectedSealedClasses = _assembly.GetTypes()
        .Where(t => t.IsClass)
        .Where(t => t.GetCustomAttribute<CompilerGeneratedAttribute>() == null)
        .Where(t => !UnsealedClasses.Contains(t))
        .Select(t => new[] { t });

    [Trait("Category", "L0")]
    [Theory]
    [MemberData(nameof(ExpectedSealedClasses))]
    public void Sould_BeSealedByDefault(Type classType)
    {
        classType.IsSealed
            .Should()
            .BeTrue();
    }
}