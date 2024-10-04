namespace PG.StarWarsGame.Components.Localisation.Repository.Content;

/// <summary>
///     Readonly representation of an <see cref="ITranslationItem" />
/// </summary>
public interface IReadOnlyTranslationItem : ITranslationItem
{
    /// <inheritdoc cref="ITranslationItem.Content" />
    new ITranslationItemContent Content { get; }
}