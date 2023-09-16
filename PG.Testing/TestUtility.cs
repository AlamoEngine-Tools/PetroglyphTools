// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PG.Testing;

public static class TestUtility
{
    private static readonly Random RandomGenerator = new Random();
        
    public static void AssertAreBinaryEquivalent(IReadOnlyList<byte> expected, IReadOnlyList<byte> actual)
    {
        Assert.AreEqual(expected.Count, actual.Count);
        for (var i = 0; i < expected.Count; i++)
        {
            Assert.AreEqual(expected[i], actual[i]);
        }
    }

    public static string GetRandomStringOfLength(int lenght)
    {
        const string allowedChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789!@$?_-";
        var chars = new char[lenght];

        for (var i = 0; i < lenght; i++)
        {
            chars[i] = allowedChars[RandomGenerator.Next(0, allowedChars.Length)];
        }

        return new string(chars);
    }

    public static void Assert_CtorException<T>(Action action) where T : Exception
    {
        Assert_CtorException(typeof(T), action);
    }

    public static void Assert_CtorException(Type type, Action action)
    {
        Assert_CtorException(type, () => action);
    }

    public static void Assert_CtorException<T>(Func<object?> action) where T : Exception
    {
        Assert_CtorException(typeof(T), action);
    }

    public static void Assert_CtorException(Type expectedException, Func<object?> action)
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

    public static Stream GetEmbeddedResource(Type type, string path)
    {
        var currentAssembly = type.Assembly;
        return currentAssembly.GetManifestResourceStream($"{currentAssembly.GetName().Name}.Resources.{path}")!;
    }
}
