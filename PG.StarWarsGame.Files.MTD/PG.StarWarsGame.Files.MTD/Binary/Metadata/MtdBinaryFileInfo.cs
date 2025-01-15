// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Buffers.Binary;
using AnakinRaW.CommonUtilities;
using PG.Commons.Utilities;
using PG.StarWarsGame.Files.Binary;
#if NETSTANDARD2_0
using AnakinRaW.CommonUtilities.Extensions;
#endif

namespace PG.StarWarsGame.Files.MTD.Binary.Metadata;

internal readonly struct MtdBinaryFileInfo : IBinary
{
    public string Name { get; }

    public uint X { get; }

    public uint Y { get; }

    public uint Width { get; }

    public uint Height { get; }

    public bool Alpha { get; }

    public int Size => 64 + sizeof(uint) * 4 + sizeof(bool);
    
    public void GetBytes(Span<byte> bytes)
    {
        var stringNameSpan = bytes.Slice(0, 64);

        var nameLength = MtdFileConstants.NameEncoding.GetBytes(Name.AsSpan(), stringNameSpan);
        var toPad = stringNameSpan.Slice(nameLength);
        toPad.Fill(0);

        var xSpan = bytes.Slice(64);
        BinaryPrimitives.WriteUInt32LittleEndian(xSpan, X);

        var ySpan = bytes.Slice(64 + sizeof(uint));
        BinaryPrimitives.WriteUInt32LittleEndian(ySpan, Y);

        var wSpan = bytes.Slice(64 + 2 * sizeof(uint));
        BinaryPrimitives.WriteUInt32LittleEndian(wSpan, Width);

        var hSpan = bytes.Slice(64 + 3 * sizeof(uint));
        BinaryPrimitives.WriteUInt32LittleEndian(hSpan, Height);

        var aSpan = bytes.Slice(64 + 4 * sizeof(uint));
        aSpan[0] = Convert.ToByte(Alpha);
    }

    public byte[] Bytes
    {
        get
        {
            var bytesArray = new byte[Size];
            GetBytes(bytesArray);
            return bytesArray;
        }
    }

    public MtdBinaryFileInfo(string name, uint x, uint y, uint width, uint height, bool alpha)
    {
        ThrowHelper.ThrowIfNullOrEmpty(name);
        StringUtilities.ValidateIsAsciiOnly(name.AsSpan());
        if (name.Length > MtdFileConstants.MaxFileNameSize)
            throw new ArgumentOutOfRangeException(nameof(name), "File name must not be larger than 63 characters.");

        if (x > int.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(x), "Image coordinates larger than int.MaxValue are not supported.");
        if (y > int.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(x), "Image coordinates larger than int.MaxValue are not supported.");
        if (width > int.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(x), "Image sizes larger than int.MaxValue are not supported.");
        if (height > int.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(x), "Image sizes larger than int.MaxValue are not supported.");

        Name = name;
        X = x;
        Y = y;
        Width = width;
        Height = height;
        Alpha = alpha;
    }
}