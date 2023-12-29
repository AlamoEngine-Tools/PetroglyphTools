// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons;
using PG.Commons.Services;

namespace PG.StarWarsGame.Files.DAT.Binary;

internal class DatBinaryServiceFactory(IServiceProvider services) : ServiceBase(services), IDatBinaryServiceFactory
{
    public IDatFileReader GetReader()
    {
        return Services.GetService<IDatFileReader>() ?? throw new LibraryInitialisationException(typeof(IDatFileReader));
    }

    public IDatBinaryConverter GetConverter()
    {
        return Services.GetService<IDatBinaryConverter>() ?? throw new LibraryInitialisationException(typeof(IDatBinaryConverter));
    }

    public IDatFileWriter GetWriter()
    {
        return Services.GetService<IDatFileWriter>() ?? throw new LibraryInitialisationException(typeof(IDatFileWriter));
    }
}