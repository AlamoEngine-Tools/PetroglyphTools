using System;
using PG.Commons.Files;
using PG.StarWarsGame.Files.ALO.Data;

namespace PG.StarWarsGame.Files.ALO.Files.Particles;

public sealed class AloParticleFile(
    AlamoParticle particle,
    AloFileInformation fileInformation,
    IServiceProvider serviceProvider)
    : PetroglyphFileHolder<AlamoParticle, AloFileInformation>(particle, fileInformation, serviceProvider), IAloParticleFile;