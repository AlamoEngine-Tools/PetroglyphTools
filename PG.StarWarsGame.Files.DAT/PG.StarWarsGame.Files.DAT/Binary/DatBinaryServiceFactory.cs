// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.Commons.Binary.File;
using PG.Commons.Common.Exceptions;
using PG.Commons.Files;
using PG.Commons.Services;
using PG.StarWarsGame.Files.DAT.Binary.Metadata;
using PG.StarWarsGame.Files.DAT.Files;

namespace PG.StarWarsGame.Files.DAT.Binary;

internal class DatBinaryServiceFactory : ServiceBase, IDatBinaryServiceFactory
{
    public IBinaryFileReader<IDatFileMetadata> GetReader()
    {
        return (IDatFileReader)(Services.GetService(typeof(IDatFileReader)) ??
                                throw new LibraryInitialisationException(
                                    $"No implementation could be found for {nameof(IDatFileReader)}."));
    }

    public IBinaryFileConverter<IDatFileMetadata, IDatFile, IFileHolderParam> GetConverter()
    {
        return (IDatFileConverter)(Services.GetService(typeof(IDatFileConverter)) ??
                                   throw new LibraryInitialisationException(
                                       $"No implementation could be found for {nameof(IDatFileConverter)}."));
    }

    public IBinaryFileWriter<IDatFileMetadata> GetWriter()
    {
        return (IDatFileWriter)(Services.GetService(typeof(IDatFileWriter)) ??
                                throw new LibraryInitialisationException(
                                    $"No implementation could be found for {nameof(IDatFileWriter)}."));
    }

    public DatBinaryServiceFactory(IServiceProvider services) : base(services)
    {
    }
}