// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using Xunit;

namespace PG.Testing;

public static class TestUtility
{
    private static readonly Random RandomGenerator = new();

    public static void AssertAreBinaryEquivalent(IReadOnlyList<byte> expected, IReadOnlyList<byte> actual)
    {
        Assert.Equal(expected.Count, actual.Count);
        for (var i = 0; i < expected.Count; i++) Assert.Equal(expected[i], actual[i]);
    }

    public static string GetRandomStringOfLength(int length)
    {
        const string allowedChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789!@$?_-";
        var chars = new char[length];

        for (var i = 0; i < length; i++) chars[i] = allowedChars[RandomGenerator.Next(0, allowedChars.Length)];

        return new string(chars);
    }

    public static Stream GetEmbeddedResource(Type type, string path)
    {
        var assembly = type.Assembly;
        var resourcePath = $"{assembly.GetName().Name}.Resources.{path}";
        return assembly.GetManifestResourceStream(resourcePath) ??
               throw new IOException($"Could not find embedded resource: '{resourcePath}'");
    }

    public static byte[] GetEmbeddedResourceAsByteArray(Type type, string path)
    {
        using var stream = GetEmbeddedResource(type, path);
        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        return ms.ToArray();
    }

    public static void CopyEmbeddedResourceToMockFilesystem(Type type, string resourcePath, string mockFilePath,
        IFileSystem fileSystem)
    {
        fileSystem.File.WriteAllBytes(mockFilePath, GetEmbeddedResourceAsByteArray(type, resourcePath));
    }
}
