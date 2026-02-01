using System;
using PG.StarWarsGame.Files.MEG.Binary;
using PG.StarWarsGame.Files.MEG.Binary.SizeCalculation;
using PG.StarWarsGame.Files.MEG.Data;

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

/// <summary>
/// Provides validation logic for MEG file data entries.
/// </summary>
/// <remarks>
/// This class implements the <see cref="IMegDataEntryValidator"/> interface to validate MEG file data entries.
/// It ensures that the entries conform to specific constraints, such as path length and entry size.
/// Derived classes can override the <see cref="ValidateCore"/> method to provide additional or specialized validation logic.
/// </remarks>
public class BinaryMegEntryValidator : IMegDataEntryValidator
{
    /// <summary>
    /// Validates the specified MEG file data entry information.
    /// </summary>
    /// <param name="entryInfo">The information about the MEG file data entry to validate.</param>
    /// <returns>
    /// A <see cref="MegDataEntryValidationResult"/> indicating the result of the validation.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="entryInfo"/> is <c>null</c>.
    /// </exception>
    /// <remarks>
    /// This method performs validation checks on the provided MEG file data entry information,
    /// such as ensuring the file path length does not exceed the maximum allowed length and
    /// that the binary entry size, including encryption, does not exceed the maximum allowed size.
    /// </remarks>
    public MegDataEntryValidationResult Validate(MegFileDataEntryBuilderInfo entryInfo)
    {
        if (entryInfo == null) 
            throw new ArgumentNullException(nameof(entryInfo));

        // Technically, empty file name is not illegal, thus we don't check here.
        if (entryInfo.FilePath.Length > MegFileConstants.MegMaxEntryPathLength)
            return new MegDataEntryValidationResult(MegDataEntryValidationStatus.InvalidPath, "Entry path too long.");

        // Necessary, because an uint.Max sized file,
        // which should get encrypted would be padded to uint.Max + 1 entry size, which then would be a long value
        var binarySize = MegSizeCalculator.GetBinaryEntrySizeWithEncryption(entryInfo);
        if (binarySize > MegFileConstants.MegMaxEntrySize)
            return new MegDataEntryValidationResult(MegDataEntryValidationStatus.InvalidPath, "Entry size too large.");

        return ValidateCore(entryInfo);
    }

    /// <summary>
    /// Validates the specified MEG file data entry and returns the validation result.
    /// </summary>
    /// <param name="entryInfo">
    /// The <see cref="MegFileDataEntryBuilderInfo"/> instance containing the details of the MEG file data entry to validate.
    /// </param>
    /// <returns>
    /// A <see cref="MegDataEntryValidationResult"/> representing the outcome of the validation.
    /// </returns>
    /// <remarks>
    /// This method is intended to be overridden in derived classes to provide specific validation logic
    /// for MEG file data entries. By default, it returns a valid result.
    /// </remarks>
    protected virtual MegDataEntryValidationResult ValidateCore(MegFileDataEntryBuilderInfo entryInfo)
    {
        return MegDataEntryValidationResult.Valid;
    }
}