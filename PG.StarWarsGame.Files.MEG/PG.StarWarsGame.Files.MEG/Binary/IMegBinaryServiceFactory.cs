using System;
using PG.Commons.Binary.File;
using PG.StarWarsGame.Files.MEG.Binary.Shared.Metadata;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Binary;

internal interface IMegBinaryServiceFactory
{
    IBinaryFileReader<IMegFileMetadata> GetReader(MegFileVersion megVersion);

    IBinaryFileReader<IMegFileMetadata> GetReader(ReadOnlySpan<byte> key, ReadOnlySpan<byte> iv);
}