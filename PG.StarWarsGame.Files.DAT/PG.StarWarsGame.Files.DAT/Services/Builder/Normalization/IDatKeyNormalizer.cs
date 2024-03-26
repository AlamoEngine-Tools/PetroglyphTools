using PG.Commons.Services.Builder;

namespace PG.StarWarsGame.Files.DAT.Services.Builder.Normalization;

/// <summary>
/// Provides methods to normalize the key for a string key-value pair to store it into a DAT file.
/// </summary>
public interface IDatKeyNormalizer : IBuilderEntryNormalizer<string>;