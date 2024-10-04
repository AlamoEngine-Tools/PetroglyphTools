namespace PG.StarWarsGame.Components.Localisation.Repository.Content;

/// <summary>
///     The raw content of a translation item.
/// </summary>
public interface ITranslationItemContent
{
    /// <summary>
    ///     The string key, usually mapped to <see cref="PG.StarWarsGame.Files.DAT.Data.DatStringEntry.Key" />
    /// </summary>
    string Key { get; set; }

    /// <summary>
    ///     The string value, usually mapped to <see cref="PG.StarWarsGame.Files.DAT.Data.DatStringEntry.Value" />
    /// </summary>
    string Value { get; set; }
}