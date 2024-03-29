using PG.Commons.Services.Builder.Normalization;

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;

/// <summary>
/// Provides methods to normalize a data entry's file path to store it into a MEG archive.
/// </summary>
public interface IMegDataEntryPathNormalizer : IBuilderEntryNormalizer<string>;