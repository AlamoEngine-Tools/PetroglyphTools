using System.Collections.Generic;

namespace PG.StarWarsGame.Files.ALO.Data;

public class AlamoModel : IAloDataContent
{
    public IList<string> Bones { get; init; }

    public ISet<string> Shaders { get; init; }

    public ISet<string> Textures { get; init; }

    public ISet<string> Proxies { get; init; }

    public void Dispose()
    {
    }
}