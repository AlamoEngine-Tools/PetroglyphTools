// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PG.Commons.Test;

public static class TestUtility
{
    private static readonly Random RANDOM_GENERATOR = new Random();
        
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
            chars[i] = allowedChars[RANDOM_GENERATOR.Next(0, allowedChars.Length)];
        }

        return new string(chars);
    }
}
