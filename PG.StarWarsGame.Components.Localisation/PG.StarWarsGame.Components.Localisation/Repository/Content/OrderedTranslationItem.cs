using System;

namespace PG.StarWarsGame.Components.Localisation.Repository.Content;

/// <inheritdoc />
public class OrderedTranslationItem : TranslationItemBase
{
    /// <inheritdoc />
    protected OrderedTranslationItem(OrderedTranslationItemId itemId, TranslationItemContent content) : base(itemId,
        content)
    {
    }

    /// <summary>
    ///     Convenience method to create a new <see cref="OrderedTranslationItem" />
    /// </summary>
    /// <param name="itemId"></param>
    /// <param name="content"></param>
    /// <returns></returns>
    public static OrderedTranslationItem Of(OrderedTranslationItemId itemId, TranslationItemContent content)
    {
        return new OrderedTranslationItem(itemId, content);
    }

    /// <summary>
    ///     Convenience method to create a new <see cref="OrderedTranslationItem" />
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static OrderedTranslationItem Of(TranslationItemContent content)
    {
        return Of(OrderedTranslationItemId.Of(content.Key) ?? throw new InvalidOperationException(), content);
    }
}
