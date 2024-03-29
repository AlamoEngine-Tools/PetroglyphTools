// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using Xunit;

namespace PG.Testing;

public static class ExceptionUtilities
{
    public static T AssertDoesNotThrowException<T>(Func<T> action)
    {
        try
        {
            return action();
        }
        catch (Exception e)
        {
            Assert.Fail($"Expected no exception to be thrown but got '{e.GetType().Name}' instead");
            return default;
        }
    }

    public static void AssertDoesNotThrowException(Action action)
    {
        AssertDoesNotThrowException(() => action);
    }
}