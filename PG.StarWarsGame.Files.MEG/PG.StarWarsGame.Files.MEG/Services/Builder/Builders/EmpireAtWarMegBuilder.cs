using System;
using PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

namespace PG.StarWarsGame.Files.MEG.Services.Builder;

/// <summary>
/// A <see cref="IMegBuilder"/> for building MEG files which are safe to use for the
/// Petroglyph game <em>Star Wars: Empire at War</em> and its extension <em>Empire at War: Forces of Corruption</em>.
/// </summary>
public sealed class EmpireAtWarMegBuilder : PetroglyphGameMegBuilder
{
    /// <summary>
    /// 
    /// </summary>
    public override IBuilderInfoValidator DataEntryValidator => EmpireAtWarMegDataEntryValidator.Instance;

    /// <summary>
    /// 
    /// </summary>
    public override IMegFileInformationValidator MegFileInformationValidator => EmpireAtWarMegFileInformationValidator.Instance;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmpireAtWarMegBuilder"/> class with a specified game path.
    /// </summary>
    /// <remarks>
    /// <paramref name="baseDirectory"/> usually is a game's or mod's ./DATA/ directory, however it can be set to any other directory.
    /// </remarks>
    /// <param name="baseDirectory">The path for this <see cref="EmpireAtWarMegBuilder"/>.</param>
    /// <param name="services">The service provider.</param>
    /// <exception cref="ArgumentNullException"><paramref name="baseDirectory"/> or <paramref name="services"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="baseDirectory"/> is empty.</exception>
    public EmpireAtWarMegBuilder(string baseDirectory, IServiceProvider services) : base(baseDirectory, services)
    {
    }
}