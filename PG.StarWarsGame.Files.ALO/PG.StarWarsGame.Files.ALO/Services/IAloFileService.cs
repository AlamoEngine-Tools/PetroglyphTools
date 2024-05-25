using System.IO;
using PG.Commons.Files;
using PG.StarWarsGame.Files.ALO.Data;
using PG.StarWarsGame.Files.ALO.Files;
using PG.StarWarsGame.Files.ALO.Files.Models;
using PG.StarWarsGame.Files.ALO.Files.Particles;

namespace PG.StarWarsGame.Files.ALO.Services;

public interface IAloFileService
{
    IAloFile<IAloDataContent, PetroglyphMegPackableFileInformation> Load(Stream stream, AloLoadOptions loadOptions = AloLoadOptions.Full);

    IAloModelFile LoadModel(Stream stream, AloLoadOptions loadOptions = AloLoadOptions.Full);

    IAloParticleFile LoadParticle(Stream stream, AloLoadOptions loadOptions = AloLoadOptions.Full);

    AloContentInfo GetAloContentInfo(Stream stream);
}