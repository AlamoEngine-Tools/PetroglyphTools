// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Buffers;
using System.Text;
using PG.StarWarsGame.Files.MEG.Binary;
#if NETSTANDARD2_0 || NETFRAMEWORK
using System.Runtime.InteropServices;
#endif

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;

/// <summary>
/// Represents a path normalizer that normalizes MEG data entry paths
/// the same way as the Empire at War Alamo engine normalizes meg entry paths
/// </summary>
public sealed class EmpireAtWarMegDataEntryPathNormalizer : PetroglyphMegDataEntryPathNormalizer
{
    /// <summary>
    /// Normalizes the specified MEG data entry path the same way as the Empire at War Alamo engine normalizes meg entry paths.
    /// </summary>
    /// <returns>The normalized entry path.</returns>
    /// <param name="entryPath">The read-only span containing the entry's file path to normalize.</param>
    public override string Normalize(ReadOnlySpan<char> entryPath)
    {
        if (entryPath.Length == 0)
            return string.Empty;

        char[]? pooledCharArray = null;
        try
        {
            var buffer = entryPath.Length > 265
                ? pooledCharArray = ArrayPool<char>.Shared.Rent(entryPath.Length)
                : stackalloc char[entryPath.Length];


            var normalizedLength = base.Normalize(entryPath, buffer);
            var normalized = buffer.Slice(0, normalizedLength);

            SplitPath(normalized, out var path, out var file);

            var sb = new StringBuilder(MegFileConstants.EawMaxEntryPathLength);
            if (path.Length > 0)
            {
                AppendRosToSb(path, sb);
                sb.Append('\\');
            }

            AppendRosToSb(file, sb);
            return sb.ToString();
        }
        finally
        {
            if (pooledCharArray is not null)
                ArrayPool<char>.Shared.Return(pooledCharArray);
        }
    }


    private static void AppendRosToSb(ReadOnlySpan<char> value, StringBuilder sb)
    {
        if (value.Length <= 0)
            return;
#if NETSTANDARD2_0 || NETFRAMEWORK
        unsafe
        {
            fixed (char* valueChars = &MemoryMarshal.GetReference(value))
                sb.Append(valueChars, value.Length);
        }
#else
        sb.Append(value);
#endif
    }


    /// <summary>
    /// Normalizes the specified MEG data entry path the same way as the Empire at War Alamo engine normalizes meg entry paths
    /// and writes the normalized path to the destination buffer.
    /// </summary>
    /// <param name="entryPath">The read-only span containing the entry's file path to normalize.</param>
    /// <param name="destination">
    /// A buffer to store the normalized path. The buffer must be large enough to hold the normalized path.
    /// </param>
    /// <returns>
    /// The number of characters written to the destination buffer after normalization.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the destination buffer is not large enough to store the normalized path.
    /// </exception>
    protected override int Normalize(ReadOnlySpan<char> entryPath, Span<char> destination)
    {
        if (entryPath.Length == 0)
            return 0;

        char[]? pooledCharArray = null;
        try
        {
            var normalizationBuffer = destination.Length > 265
                ? pooledCharArray = ArrayPool<char>.Shared.Rent(destination.Length)
                : stackalloc char[destination.Length];

            var normalizedLength = base.Normalize(entryPath, normalizationBuffer);
            var normalized = normalizationBuffer.Slice(0, normalizedLength);

            SplitPath(normalized, out var path, out var file);

            var pos = 0;
            if (path.Length > 0)
            {
                path.CopyTo(destination);
                destination[path.Length] = '\\';

                pos = path.Length + 1;
            }

            var fileSpan = destination.Slice(pos);
            file.CopyTo(fileSpan);

            return pos + file.Length;
        }
        finally
        {
            if (pooledCharArray is not null)
                ArrayPool<char>.Shared.Return(pooledCharArray);
        }
    }

    private static void SplitPath(ReadOnlySpan<char> input, out ReadOnlySpan<char> pathPart, out ReadOnlySpan<char> filePart)
    {
        var fileStartIndex = input.LastIndexOf('\\');
        if (fileStartIndex == -1)
        {
            filePart = input;
            pathPart = ReadOnlySpan<char>.Empty;
            return;
        }

        var pathStart = 0;
        var colon = input.IndexOf(':');
        if (colon != -1)
            pathStart = colon;

        if (input[pathStart] == '.')
            ++pathStart;
        if (input[pathStart] == '\\')
            ++pathStart;


        var length = (int)Math.Min(input.Length - pathStart, (uint)(fileStartIndex - pathStart));
        pathPart = input.Slice(pathStart, length);

        filePart = input.Slice(fileStartIndex + 1);
    }
}