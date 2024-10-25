using PG.Commons.Data;

namespace PG.StarWarsGame.Components.Localisation.Repository.Content;

/// <summary>
///     Item ID for ordered translation items. Equivalent to the raw string key from a sorted DAT file.
/// </summary>
public record OrderedTranslationItemId : RootIdBase<string>, ITranslationItemId
{
    /// <inheritdoc />
    protected OrderedTranslationItemId(string rawId) : base(rawId)
    {
    }

    /// <summary>
    ///     Convenience method to create a new <see cref="OrderedTranslationItemId" />
    /// </summary>
    /// <param name="rawId"></param>
    /// <returns></returns>
    public static OrderedTranslationItemId? Of(string rawId)
    {
        return string.IsNullOrWhiteSpace(rawId) ? null : new OrderedTranslationItemId(rawId);
    }
}
