using System;
using PG.StarWarsGame.Files.MEG.Binary;
using PG.StarWarsGame.Files.MEG.Binary.SizeCalculation;
using PG.StarWarsGame.Files.MEG.Data;

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

/// <summary>
/// Represents a validator for MEG data entries that validates data entries
/// to be compliant with the binary specifications of MEG files
/// </summary>
public class BinaryMegEntryValidator : IMegDataEntryValidator
{
    /// <inheritdoc/>
    /// <remarks>
    /// This method performs validation checks on the specified <paramref name="dataEntry"/> based on the binary specification of MEG files.
    /// </remarks>
    public MegDataEntryValidationResult Validate(MegDataEntryBuilderInfo dataEntry)
    {
        if (dataEntry == null) 
            throw new ArgumentNullException(nameof(dataEntry));

        // Technically, empty file name is not illegal, thus we don't check here.
        if (dataEntry.EntryPath.Length > MegFileConstants.MegMaxEntryPathLength)
            return new MegDataEntryValidationResult(MegDataEntryValidationStatus.InvalidPath, "Entry path too long.");

        // Necessary, because an uint.Max sized entry,
        // which should get encrypted would be padded to uint.Max + 1, which then would be a long value
        var binarySize = MegSizeCalculator.GetBinaryEntrySizeWithEncryption(dataEntry);
        if (binarySize > MegFileConstants.MegMaxEntrySize)
            return new MegDataEntryValidationResult(MegDataEntryValidationStatus.InvalidPath, "Entry size too large.");

        return ValidateCore(dataEntry);
    }

    /// <summary>
    /// Validates the specified MEG data entry.
    /// </summary>
    /// <param name="dataEntry">The MEG data entry to validate.</param>
    /// <returns>A <see cref="MegDataEntryValidationResult"/> indicating the outcome of the validation.</returns>
    /// <remarks>
    /// This method is intended to be overridden in derived classes to provide specific validation logic
    /// for MEG file data entries. By default, it returns a valid result.
    /// </remarks>
    protected virtual MegDataEntryValidationResult ValidateCore(MegDataEntryBuilderInfo dataEntry)
    {
        return MegDataEntryValidationResult.Valid;
    }
}