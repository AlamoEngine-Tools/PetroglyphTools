using System;

namespace PG.StarWarsGame.Components.Localisation.Repository.Content;

/// <inheritdoc />
public class OrderedTranslationItem : TranslationItemBase
{
    /// <inheritdoc />
    protected OrderedTranslationItem(OrderedTranslationItemId itemId, TranslationItemContent content,
        ITranslationItem.TranslationItemSource source, bool overwritten) : base(itemId, content, source, overwritten)
    {
    }

    /// <summary>
    ///     Convenience method to create a new <see cref="OrderedTranslationItem" />
    /// </summary>
    /// <param name="itemId"></param>
    /// <param name="content"></param>
    /// <param name="source"></param>
    /// <param name="overwritten"></param>
    /// <returns></returns>
    public static OrderedTranslationItem Of(OrderedTranslationItemId itemId, TranslationItemContent content,
        ITranslationItem.TranslationItemSource source, bool overwritten)
    {
        return new OrderedTranslationItem(itemId, content, source, overwritten);
    }

    /// <summary>
    ///     Convenience method to create a new <see cref="OrderedTranslationItem" />
    /// </summary>
    /// <param name="content"></param>
    /// <param name="source"></param>
    /// <param name="overwritten"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static OrderedTranslationItem Of(TranslationItemContent content,
        ITranslationItem.TranslationItemSource source = ITranslationItem.TranslationItemSource.Mod,
        bool overwritten = false)
    {
        return Of(OrderedTranslationItemId.Of(content.Key) ?? throw new InvalidOperationException(), content, source,
            overwritten);
    }
}