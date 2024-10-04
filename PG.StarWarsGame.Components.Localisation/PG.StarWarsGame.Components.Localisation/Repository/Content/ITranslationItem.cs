namespace PG.StarWarsGame.Components.Localisation.Repository.Content;

/// <summary>
///     The basic representation of a translation item.
/// </summary>
public interface ITranslationItem
{
    /// <summary>
    ///     The source of the translation item.
    /// </summary>
    enum TranslationItemSource
    {
        /// <summary>
        ///     A translation item available in the base game.
        /// </summary>
        BaseGame,

        /// <summary>
        ///     A translation item added by the expansion
        /// </summary>
        Expansion,

        /// <summary>
        ///     A translation item added by a mod.
        /// </summary>
        Mod
    }

    /// <summary>
    ///     ID
    /// </summary>
    ITranslationItemId ItemId { get; }

    /// <summary>
    ///     The acutal translation content.
    /// </summary>
    ITranslationItemContent Content { get; set; }

    /// <summary>
    ///     The source of the item.
    /// </summary>
    TranslationItemSource Source { get; }

    /// <summary>
    ///     If an item is originally present in the <see cref="TranslationItemSource.BaseGame" /> or the
    ///     <see cref="TranslationItemSource.Expansion" />, but also contained in either the
    ///     <see cref="TranslationItemSource.Expansion" /> or the <see cref="TranslationItemSource.Mod" /> with a different
    ///     value the item counts as overwritten.
    /// </summary>
    bool Overwritten { get; }
}