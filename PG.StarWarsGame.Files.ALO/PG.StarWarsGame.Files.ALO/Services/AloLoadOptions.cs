using System;

namespace PG.StarWarsGame.Files.ALO.Services;

[Flags]
public enum AloLoadOptions
{
    /// <summary>
    /// Loads the entire file.
    /// </summary>
    /// <remarks>
    /// This option can be used if the file shall be rendered.
    /// </remarks>
    Full = 0,
    /// <summary>
    ///  Extracts only assets from the model/particle (which are textures and proxies)
    /// </summary>
    Assets = 1,
    /// <summary>
    /// If the file is a model, this option gets the list of bones from the model.
    /// </summary>
    Bones = 2,
}