namespace PG.StarWarsGame.Components.Localisation.Repository.Content;

/// <summary>
///     The basic representation of a translation item.
/// </summary>
public interface ITranslationItem
{
    /// <summary>
    ///     ID
    /// </summary>
    ITranslationItemId ItemId { get; }

    /// <summary>
    ///     The acutal translation content.
    /// </summary>
    ITranslationItemContent Content { get; set; }
}
