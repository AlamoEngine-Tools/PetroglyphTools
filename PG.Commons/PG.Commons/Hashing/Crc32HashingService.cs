// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Buffers;
using System.Globalization;
using System.IO;
using System.Text;
using AnakinRaW.CommonUtilities.Extensions;
using AnakinRaW.CommonUtilities.Hashing;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Utilities;

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

    public unsafe Crc32 GetCrc32(string value, Encoding encoding)
    {
        Span<byte> checksum = stackalloc byte[sizeof(Crc32)];
        _hashingService.GetHash(value, encoding, checksum, Crc32Hashing.Crc32HashKey);
        return new Crc32(checksum);
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

    public Crc32 GetCrc32(ReadOnlySpan<char> value, Encoding encoding)
    {
        byte[]? pooledByteArray = null;
        try
        {
            var expectedSize = encoding.GetByteCountPG(value.Length);

            var buffer = expectedSize > 260
                ? pooledByteArray = ArrayPool<byte>.Shared.Rent(expectedSize)
                : stackalloc byte[expectedSize];

            var bytes = encoding.GetBytesReadOnly(value, buffer);

            return GetCrc32(bytes);
        }
        finally
        {
            if (pooledByteArray is not null)
                ArrayPool<byte>.Shared.Return(pooledByteArray);
        }
    }

    public Crc32 GetCrc32Upper(ReadOnlySpan<char> value, Encoding encoding)
    {
        char[]? pooledCharArray = null;
        try
        {
            var buffer = value.Length > 260
                ? pooledCharArray = ArrayPool<char>.Shared.Rent(value.Length)
                : stackalloc char[value.Length];

            var bytesRead = value.ToUpper(buffer, CultureInfo.InvariantCulture);

            return GetCrc32(buffer.Slice(0, bytesRead), encoding);
        }
        finally
        {
            if (pooledCharArray is not null)
                ArrayPool<char>.Shared.Return(pooledCharArray);
        }
    }
}