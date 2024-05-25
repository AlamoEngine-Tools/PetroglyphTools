using System;
using System.IO;
using PG.StarWarsGame.Files.ALO.Binary.Reader;
using PG.StarWarsGame.Files.ALO.Data;
using PG.StarWarsGame.Files.ALO.Files;
using PG.StarWarsGame.Files.ALO.Services;

namespace PG.StarWarsGame.Files.ALO.Binary;

internal class AloFileReaderFactory(IServiceProvider serviceProvider) : IAloFileReaderFactory
{
    public IAloFileReader<IAloDataContent> GetReader(AloContentInfo contentInfo, Stream dataStream, AloLoadOptions loadOptions)
    {
        if (contentInfo.Type == AloType.Model)
            return new ModelFileReader(loadOptions, dataStream);
        if (contentInfo.Version == AloVersion.V1)
            return new ParticleReaderV1(loadOptions, dataStream);
        return new ParticleReaderV2(loadOptions, dataStream);
    }
}