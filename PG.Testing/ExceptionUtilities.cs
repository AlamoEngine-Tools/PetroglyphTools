// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

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
            throw new AssertFailedException("Expected an exception but none was thrown.");
        var exceptionType = exception.GetType();
        if (!exceptionTypes.Contains(exceptionType))
            throw new AssertFailedException($"Caught wrong exception: {exceptionType.Name}");
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

    public static void AssertThrowsException_IgnoreTargetInvocationException<T>(Action action) where T : Exception
    {
        AssertThrowsException_IgnoreTargetInvocationException(typeof(T), action);
    }

    public static void AssertThrowsException_IgnoreTargetInvocationException(Type type, Action action)
    {
        AssertThrowsException_IgnoreTargetInvocationException(type, () => action);
    }

    public static void AssertThrowsException_IgnoreTargetInvocationException<T>(Func<object?> action) where T : Exception
    {
        AssertThrowsException_IgnoreTargetInvocationException(typeof(T), action);
    }

    public static void AssertThrowsException_IgnoreTargetInvocationException(Type expectedException, Func<object?> action)
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
                Assert.Fail($"Expected exception of type {expectedException.Name} but got {e.InnerException?.GetType().Name}");
            return;
        }
        catch (Exception e)
        {
            if (e.GetType() == expectedException)
                return;
            Assert.Fail($"Expected exception of type {expectedException.Name} but got {e.GetType().Name}");
        }
        Assert.Fail($"Excepted exception of type {expectedException.Name} but non was thrown.");
    }
}