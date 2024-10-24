namespace PG.StarWarsGame.Components.Localisation.Repository.Content;

/// <inheritdoc />
public abstract class TranslationItemBase : ITranslationItem
{
    /// <summary>
    ///     .ctor
    /// </summary>
    /// <param name="itemId"></param>
    /// <param name="content"></param>
    protected TranslationItemBase(ITranslationItemId itemId, TranslationItemContent content)
    {
        ItemId = itemId;
        Content = content;
    }

    /// <inheritdoc />
    public ITranslationItemContent Content { get; set; }

    /// <inheritdoc />
    public ITranslationItemId ItemId { get; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{GetType().Name}[{ItemId}: {Content}]";
    }
}
