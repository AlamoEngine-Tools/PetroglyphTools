using System.IO;
using PG.StarWarsGame.Files.ALO.Data;
using PG.StarWarsGame.Files.ALO.Services;

namespace PG.StarWarsGame.Files.ALO.Binary.Reader;

internal class ParticleReaderV1(AloLoadOptions loadOptions, Stream stream) : AloFileReader<AlamoParticle>(loadOptions, stream)
{
    public override AlamoParticle Read()
    {
        return new AlamoParticle();
    }
}