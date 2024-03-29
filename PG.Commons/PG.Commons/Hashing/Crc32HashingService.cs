// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
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
}