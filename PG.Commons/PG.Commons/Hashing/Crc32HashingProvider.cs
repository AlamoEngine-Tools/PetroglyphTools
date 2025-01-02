// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AnakinRaW.CommonUtilities.Hashing;

namespace PG.Commons.Hashing;

/// <summary>
/// A service to compute CRC32 checksum which is used throughout the Alamo Engine.
/// </summary>
internal class Crc32HashingProvider : IHashAlgorithmProvider
{
    public HashTypeKey SupportedHashType => Crc32Hashing.Crc32HashKey;

    public int HashData(ReadOnlySpan<byte> source, Span<byte> destination)
    {
        return System.IO.Hashing.Crc32.Hash(source, destination);
    }

    public int HashData(Stream source, Span<byte> destination)
    {
        var h = new System.IO.Hashing.Crc32();
        h.Append(source);
        return h.GetHashAndReset(destination);
    }

    public async ValueTask<int> HashDataAsync(Stream source, Memory<byte> destination, CancellationToken cancellation = default)
    {
        var h = new System.IO.Hashing.Crc32();
        await h.AppendAsync(source, cancellation).ConfigureAwait(false);
        return h.GetHashAndReset(destination.Span);
    }
}