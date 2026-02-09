// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Files.MEG.Binary;
using PG.StarWarsGame.Files.MEG.Binary.Size;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Files;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

/// <summary>
/// Validates whether a <see cref="MegFileInformation"/> is compliant to the MEG specification.
/// </summary>
public class BinaryMegFileInformationValidator : IMegFileInformationValidator
{
    /// <summary>
    /// The service provider.
    /// </summary>
    protected readonly IServiceProvider ServiceProvider;

    /// <summary>
    /// Gets the MEG file versions supported by this validator.
    /// </summary>
    /// <value>
    /// A read-only collection containing the supported versions of the .MEG file format
    /// that are validated by this implementation.
    /// For <see cref="BinaryMegFileInformationValidator"/>, this includes all versions
    /// </value>
    protected virtual IReadOnlyCollection<MegFileVersion> SupportedVersions { get; } 
        = [MegFileVersion.V1, MegFileVersion.V2, MegFileVersion.V3];

    /// <summary>
    /// Gets the maximum allowed size for a MEG file in bytes, as defined by the MEG specification.
    /// </summary>
    /// <value>
    /// The maximum allowed size for a MEG file in bytes, as defined by the MEG specification.
    /// </value>
    protected virtual uint MaxMegFileSize { get; } =
        MaxMegSizeProvider.GetMegMaxSize(MaxMegSizeMode.Binary).MaxFileSize;

    /// <summary>
    /// Gets the maximum allowed size for a MEG data entry in bytes, as defined by the MEG specification.
    /// </summary>
    /// <value>
    /// The maximum allowed size for a MEG data entry in bytes, as defined by the MEG specification.
    /// </value>
    protected virtual uint MaxMegEntrySize { get; } =
        MaxMegSizeProvider.GetMegMaxSize(MaxMegSizeMode.Binary).MaxEntrySize;

    /// <summary>
    /// Initializes a new instance of the <see cref="BinaryMegFileInformationValidator"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider used to resolve dependencies.</param>
    /// <exception cref="ArgumentNullException"><paramref name="serviceProvider"/> is <see langword="null"/>.</exception>
    public BinaryMegFileInformationValidator(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }
    
    /// <inheritdoc />
    /// <remarks>
    /// This method performs validation checks on the specified <see cref="MegFileInformation"/>
    /// based on the binary specification of MEG files, using <paramref name="dataEntries"/> as context for the validation.
    /// </remarks>
    public MegFileInfoValidationResult Validate(MegFileInformation fileInformation, IReadOnlyCollection<MegDataEntryBuilderInfo> dataEntries)
    {
        if (fileInformation == null) 
            throw new ArgumentNullException(nameof(fileInformation));
        if (dataEntries == null)
            throw new ArgumentNullException(nameof(dataEntries));

        if (!SupportedVersions.Contains(fileInformation.FileVersion))
            return new MegFileInfoValidationResult(false, $"MEG version {fileInformation.FileVersion} is currently not supported.");

        var isEncrypted = dataEntries.Any(e => e.Encrypted);
        var hasEncryptionData = fileInformation.HasEncryption;

        if (isEncrypted && !hasEncryptionData)
            return new MegFileInfoValidationResult(false, "Encryption data must be provided for encrypted MEG archives.");

        if (!isEncrypted && hasEncryptionData)
            return new MegFileInfoValidationResult(false, "No encryption data must be provided for non-encrypted MEG archives.");

        IMegSizeCalculator sizeCalculator;
        try
        {
            sizeCalculator = ServiceProvider.GetRequiredService<IMegBinaryServiceFactory>()
                .GetMegSizeCalculator(fileInformation.FileVersion);
        }
        catch (NotImplementedException)
        {
            return new MegFileInfoValidationResult(false, $"MEG version {fileInformation.FileVersion} is currently not supported.");
        }

        try
        {
            foreach (var entry in dataEntries)
            {
                entry.RefreshSize();

                // Necessary, because an uint.Max sized entry,
                // which should get encrypted would be padded to uint.Max + 1, which then would be a long value
                var binarySize = MegSizeCalculator.GetBinaryEntrySizeWithEncryption(entry);

                if (binarySize > MaxMegEntrySize)
                    return new MegFileInfoValidationResult(false, "A MEG entry exceeds the maximum allowed size.");

                sizeCalculator.AddEntry(entry);
            }
            
            if (sizeCalculator.CurrentSize > MaxMegFileSize)
                return new MegFileInfoValidationResult(false, 
                    "The total size of the MEG entries exceeds the maximum allowed size.");
        }
        catch (FileNotFoundException)
        {
            return new MegFileInfoValidationResult(false, "One or more MEG entries reference files that could not be found.");
        }
        catch (MegEntrySizeException)
        {
            return new MegFileInfoValidationResult(false, "A MEG entry exceeds the maximum allowed size.");
        }

        return ValidateCore(fileInformation, dataEntries);
    }

    /// <summary>
    /// Validates the specified MEG file information.
    /// </summary>
    /// <param name="fileInformation">The MEG file information to validate.</param>
    /// <param name="dataEntries">The collection of data entries associated with the MEG file.</param>
    /// <returns>A <see cref="MegFileInfoValidationResult"/> indicating the outcome of the validation.</returns>
    /// <remarks>
    /// This method is intended to be overridden in derived classes to provide specific validation logic
    /// for MEG file data entries. By default, it returns a valid result.
    /// </remarks>
    protected virtual MegFileInfoValidationResult ValidateCore(MegFileInformation fileInformation, IReadOnlyCollection<MegDataEntryBuilderInfo> dataEntries)
    {
        return MegFileInfoValidationResult.Valid;
    }
}