using System;
using PG.Commons.Files;
using PG.StarWarsGame.Files.ALO.Data;

namespace PG.StarWarsGame.Files.ALO.Files.Particles;

public sealed class AloParticleFile : PetroglyphFileHolder<AlamoParticle, AloFileInformation>, IAloParticleFile
{
    public AloParticleFile(AlamoParticle particle,
        AloFileInformation fileInformation,
        IServiceProvider serviceProvider) : base(particle, fileInformation, serviceProvider)
    {
        if (fileInformation.IsModel)
            throw new ArgumentException("file information is referencing a model.", nameof(fileInformation));
    }
}