using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DevTask.Cli.Tests.TestHelpers.Extensions;

public static class ParametersCheckExtensions
{
    public static bool AreOfExpectedTypes(this IEnumerable<ParameterInfo> parametersInfo, IEnumerable<Type> expectedParametersTypes)
    {
        if (parametersInfo.Count() != expectedParametersTypes.Count())
        {
            return false;
        }

        return parametersInfo
            .OrderBy(p => p.Position)
            .Select(p => p.ParameterType)
            .SequenceEqual(expectedParametersTypes);
    }
}
