// // Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// // Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PG.Testing;

public static class ExceptionUtilities
{
    public static void AssertDoesNotThrowException<T>(Func<T> action)
    {
        AssertDoesNotThrowException(() =>
        {
            action();
        });
    }

    public static void AssertDoesNotThrowException(Action action)
    {
        try
        {
            action();
        }
        catch (Exception e)
        {
            throw new AssertFailedException(
                $"Expected no exception to be thrown but got '{e.GetType().Name}' instead", e);
        }
    }



    public static void AssertThrowsException<T>(Type type, Func<T> action)
    {
        AssertThrowsException(type, () =>
        {
            action();
        });
    }

    public static void AssertThrowsException(Type type, Action action)
    {
        var exception = Record(action);
        if (exception is null)
            throw new AssertFailedException($"Expected an exception of type '{type.Name}' but none was thrown.");
        if (exception.GetType() != type)
            throw new AssertFailedException($"Expected an exception of type '{type.Name}' but '{exception.GetType().Name}' was thrown.");
    }

    public static void AssertThrows(Type[] exceptionTypes, Action testCode)
    {
        var exception = Record(testCode);
        if (exception is null)
            throw new AssertFailedException("Expected an exception but got null.");
        var exceptionType = exception.GetType();
        if (!exceptionTypes.Contains(exceptionType))
            throw new AssertFailedException("Caught wrong exception.");
    }

    public static Exception? Record(Action testCode)
    {
        try
        {
            testCode();
            return null;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public static void VSTesting_Assert_CtorException<T>(Action action) where T : Exception
    {
        VSTesting_Assert_CtorException(typeof(T), action);
    }

    public static void VSTesting_Assert_CtorException(Type type, Action action)
    {
        VSTesting_Assert_CtorException(type, () => action);
    }

    public static void VSTesting_Assert_CtorException<T>(Func<object?> action) where T : Exception
    {
        VSTesting_Assert_CtorException(typeof(T), action);
    }

    public static void VSTesting_Assert_CtorException(Type expectedException, Func<object?> action)
    {
        if (expectedException.IsAssignableFrom(typeof(Exception)))
            throw new ArgumentException("Type argument must be assignable from System.Exception", nameof(expectedException));
        try
        {
            action();
        }
        catch (TargetInvocationException e)
        {
            if (e.InnerException?.GetType() != expectedException)
                Assert.Fail();
            return;
        }
        Assert.Fail();
    }
}