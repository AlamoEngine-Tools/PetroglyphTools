using System;

namespace PG.StarWarsGame.Files.MEG.Services.Builders;

/// <summary>
/// A <see cref="IMegBuilder"/> for building MEG files which are safe to use for the
/// Petroglyh game <em>Star Wars: Empire at War</em> and its extension <em>Empire at War: Forces of Corruption</em>.
/// </summary>
public sealed class EmpireAtWarMegBuilder : PetroglyphGameMegBuilder
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="baseDirectory"></param>
    /// <param name="services"></param>
    public EmpireAtWarMegBuilder(string baseDirectory, IServiceProvider services) : base(baseDirectory, services)
    {
    }
}