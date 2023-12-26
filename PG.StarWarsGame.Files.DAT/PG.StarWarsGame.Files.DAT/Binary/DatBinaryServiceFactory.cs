// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.Commons.Common.Exceptions;
using PG.Commons.Services;

namespace PG.StarWarsGame.Files.DAT.Binary;

internal class DatBinaryServiceFactory(IServiceProvider services) : ServiceBase(services), IDatBinaryServiceFactory
{
    public IDatFileReader GetReader()
    {
        return (IDatFileReader)(Services.GetService(typeof(IDatFileReader)) ??
                                throw new LibraryInitialisationException(
                                    $"No implementation could be found for {nameof(IDatFileReader)}."));
    }

    public IDatBinaryConverter GetConverter()
    {
        return (IDatBinaryConverter)(Services.GetService(typeof(IDatBinaryConverter)) ??
                                   throw new LibraryInitialisationException(
                                       $"No implementation could be found for {nameof(IDatBinaryConverter)}."));
    }

    public IDatFileWriter GetWriter()
    {
        return (IDatFileWriter)(Services.GetService(typeof(IDatFileWriter)) ??
                                throw new LibraryInitialisationException(
                                    $"No implementation could be found for {nameof(IDatFileWriter)}."));
    }
}