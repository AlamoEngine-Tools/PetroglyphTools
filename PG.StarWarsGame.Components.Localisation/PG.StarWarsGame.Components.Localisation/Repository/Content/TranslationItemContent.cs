namespace PG.StarWarsGame.Components.Localisation.Repository.Content;

/// <inheritdoc />
public record TranslationItemContent : ITranslationItemContent
{
    /// <inheritdoc />
    public required string Key { get; init; }

    /// <inheritdoc />
    public string? Value { get; init; }
}
