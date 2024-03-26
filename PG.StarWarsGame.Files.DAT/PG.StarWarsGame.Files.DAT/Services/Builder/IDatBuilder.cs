using System;
using System.Collections.Generic;
using System.IO;
using PG.StarWarsGame.Files.DAT.Data;
using PG.StarWarsGame.Files.DAT.Files;
using PG.StarWarsGame.Files.DAT.Services.Builder.Normalization;
using PG.StarWarsGame.Files.DAT.Services.Builder.Validation;

namespace PG.StarWarsGame.Files.DAT.Services.Builder;

/// <summary>
/// Service to create DAT files ensuring custom validation and normalization rules.
/// </summary>
public interface IDatBuilder
{
    /// <summary>
    /// Gets the key sort order of the DATs created by the <see cref="IDatBuilder"/>.
    /// </summary>
    DatFileType TargetKeySortOrder { get; }

    /// <summary>
    /// Gets a value indicating whether the <see cref="IDatBuilder"/> normalizes a key before adding it.
    /// </summary>
    bool NormalizesKeys { get; }

    /// <summary>
    /// Gets a value indicating how the <see cref="IDatBuilder"/> treats an already existing key.
    /// </summary>
    BuilderOverrideKind KeyOverwriteBehavior { get; }

    /// <summary>
    /// Gets a list of all entries in the order of how they have been added to the <see cref="IDatBuilder"/>.
    /// </summary>
    IReadOnlyList<DatStringEntry> DataEntries { get; }

    /// <summary>
    /// Gets a list of all entries sorted by their CRC32 checksum.
    /// </summary>
    IReadOnlyList<DatStringEntry> SortedEntries { get; }

    /// <summary>
    /// Gets the key validator for this <see cref="IDatBuilder"/>.
    /// </summary>
    IDatKeyValidator KeyValidator { get; }

    /// <summary>
    /// Gets the key normalizer or <see langword="null"/> if no normalizer is specified.
    /// </summary>
    IDatKeyNormalizer? KeyNormalizer { get; }

    /// <summary>
    /// Adds a key-value pair to the <see cref="IDatBuilder"/> and returns a status information to indicate whether the entry was successfully added. 
    /// </summary>
    /// <remarks>
    /// The actual added key might be different to <paramref name="key"/> depending whether this <see cref="IDatBuilder"/> normalizes keys before adding, or not.
    /// </remarks>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <returns>The result of this operation.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> or <paramref name="value"/> is <see langword="null"/>.</exception>
    AddEntryResult AddEntry(string key, string value);

    /// <summary>
    /// Removes the first occurrence of a specific object from the <see cref="IDatBuilder"/>.
    /// </summary>
    /// <param name="entry">The entry to remove from the <see cref="IDatBuilder"/>.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="entry"/> was successfully removed from the <see cref="IDatBuilder"/>; otherwise, <see langword="false"/>.
    /// This method also returns <see langword="false"/> if <paramref name="entry"/> is not found in the original <see cref="IDatBuilder"/>.
    /// </returns>
    bool Remove(DatStringEntry entry);

    /// <summary>
    /// Removes all entries with the specified key.
    /// </summary>
    /// <param name="key">The key to remove.</param>
    /// <returns><see langword="true"/> if <paramref name="key"/> was successfully removed from the <see cref="IDatBuilder"/>; otherwise, <see langword="false"/>.
    /// This method also returns <see langword="false"/> if <paramref name="key"/> is not found in the original <see cref="IDatBuilder"/>.</returns>
    bool RemoveAllKeys(string key);

    /// <summary>
    /// Removes all entries from the <see cref="IDatBuilder"/>.
    /// </summary>
    void Clear();

    /// <summary>
    /// Builds a .DAT file from this instance.
    /// </summary>
    /// <param name="fileInformation">The file parameters of the to be created DTA file.</param>
    /// <param name="overwrite">When set to <see langword="true"/> an existing DAT file will be overwritten; otherwise an <see cref="IOException"/> is thrown if the file already exists.</param>
    /// <exception cref="ArgumentNullException"><paramref name="fileInformation"/> is <see langword="null"/>.</exception>
    /// <exception cref="IOException">The DAT file could not be created due to an IO error.</exception>
    void Build(DatFileInformation fileInformation, bool overwrite);

    /// <summary>
    /// Checks whether the specified key is valid for this <see cref="IDatBuilder"/>.
    /// </summary>
    /// <param name="key">The key to validate</param>
    /// <returns><see langword="true"/> if the passed file information are valid; otherwise, <see langword="false"/>.</returns>
    bool IsKeyValid(string key);
}