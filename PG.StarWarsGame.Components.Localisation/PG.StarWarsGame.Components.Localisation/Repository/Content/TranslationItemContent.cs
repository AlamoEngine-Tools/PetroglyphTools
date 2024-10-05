namespace PG.StarWarsGame.Components.Localisation.Repository.Content;

/// <inheritdoc />
public record TranslationItemContent : ITranslationItemContent
{
    /// <inheritdoc />
    public required string Key { get; set; }

    /// <inheritdoc />
    public string? Value { get; set; }
}