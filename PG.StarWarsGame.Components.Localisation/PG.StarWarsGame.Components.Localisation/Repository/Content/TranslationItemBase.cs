namespace PG.StarWarsGame.Components.Localisation.Repository.Content;

/// <inheritdoc />
public abstract class TranslationItemBase : ITranslationItem
{
    /// <summary>
    ///     .ctor
    /// </summary>
    /// <param name="itemId"></param>
    /// <param name="content"></param>
    /// <param name="source"></param>
    /// <param name="overwritten"></param>
    protected TranslationItemBase(ITranslationItemId itemId, TranslationItemContent content,
        ITranslationItem.TranslationItemSource source, bool overwritten)
    {
        ItemId = itemId;
        Content = content;
        Source = source;
        Overwritten = overwritten;
    }

    /// <inheritdoc />
    public ITranslationItemContent Content { get; set; }

    /// <inheritdoc />
    public ITranslationItemId ItemId { get; }

    /// <inheritdoc />
    public ITranslationItem.TranslationItemSource Source { get; }

    /// <inheritdoc />
    public bool Overwritten { get; }
}