using System.Collections.Generic;
using PG.StarWarsGame.Files.MEG.Data;

namespace PG.StarWarsGame.Files.MEG.Services.Builder;

/// <summary>
/// Represents a single MEG file part with its entries and expected file size.
/// </summary>
internal sealed class MegFilePart(ICollection<MegDataEntryBuilderInfo> entries, uint expectedFileSize)
{
    public ICollection<MegDataEntryBuilderInfo> Entries { get; } = entries;
    
    public uint ExpectedFileSize { get; } = expectedFileSize;
}