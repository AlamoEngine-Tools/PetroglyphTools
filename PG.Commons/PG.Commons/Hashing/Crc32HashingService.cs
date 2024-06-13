// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Buffers;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using AnakinRaW.CommonUtilities.Hashing;
using Microsoft.Extensions.DependencyInjection;

namespace PG.Commons.Hashing;

internal class Crc32HashingService : ICrc32HashingService
{
    private readonly IHashingService _hashingService;

    internal Crc32HashingService(IServiceProvider serviceProvider)
    {
        if (serviceProvider == null)
            throw new ArgumentNullException(nameof(serviceProvider));
        _hashingService = serviceProvider.GetRequiredService<IHashingService>();
    }

    public Crc32 GetCrc32(string value, Encoding encoding)
    {
        return GetCrc32(value.AsSpan(), encoding);
    }

    public unsafe Crc32 GetCrc32(ReadOnlySpan<byte> data)
    {
        Span<byte> checksum = stackalloc byte[sizeof(Crc32)];
        _hashingService.GetHash(data, checksum, Crc32Hashing.Crc32HashKey);
        return new Crc32(checksum);
    }

    public unsafe Crc32 GetCrc32(Stream data)
    {
        Span<byte> checksum = stackalloc byte[sizeof(Crc32)];
        _hashingService.GetHash(data, checksum, Crc32Hashing.Crc32HashKey);
        return new Crc32(checksum);
    }

    public unsafe Crc32 GetCrc32(ReadOnlySpan<char> value, Encoding encoding)
    {
        Span<byte> checksum = stackalloc byte[sizeof(Crc32)];
        _hashingService.GetHash(value, checksum, encoding, Crc32Hashing.Crc32HashKey);
        return new Crc32(checksum);
    }

    public Crc32 GetCrc32Upper(ReadOnlySpan<char> value, Encoding encoding)
    {
        char[]? pooledCharArray = null;
        try
        {
            var buffer = value.Length > 265
                ? pooledCharArray = ArrayPool<char>.Shared.Rent(value.Length)
                : stackalloc char[value.Length];

            var bytesRead = value.ToUpper(buffer, CultureInfo.InvariantCulture);
            Debug.Assert(bytesRead == value.Length);

            return GetCrc32(buffer.Slice(0, bytesRead), encoding);
        }
        finally
        {
            if (pooledCharArray is not null)
                ArrayPool<char>.Shared.Return(pooledCharArray);
        }
    }
}