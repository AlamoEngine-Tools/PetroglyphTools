using System;
using PG.Commons.Files;
using PG.StarWarsGame.Files.ALO.Data;

namespace PG.StarWarsGame.Files.ALO.Files.Models;

public sealed class AloModelFile : PetroglyphFileHolder<AlamoModel, AloFileInformation>, IAloModelFile
{
    public AloModelFile(AlamoModel model,
        AloFileInformation fileInformation,
        IServiceProvider serviceProvider) : base(model, fileInformation, serviceProvider)
    {
        if (fileInformation.IsParticle)
            throw new ArgumentException("file information is referencing a particle.", nameof(fileInformation));
    }
}