using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DevTask.Cli.Tests.TestHelpers.Extensions;

public static class PropertyCheckExtensions
{
    public static bool IsInitOnly(this PropertyInfo propertyInfo)
    {
        if (propertyInfo.GetSetMethod() is MethodInfo setMethod)
        {
            return setMethod.ReturnParameter.GetRequiredCustomModifiers().Contains(typeof(IsExternalInit));
        }

        return false;
    }
}
