// Copyright (c) Alamo Engine To.ols and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Files;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Files.MEG.Binary;
using PG.StarWarsGame.Files.MEG.Binary.Size;

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
    /// Gets the maximum allowed size for a MEG file in bytes, as defined by the MEG specification.
    /// </summary>
    /// <value>
    /// The maximum allowed size for a MEG file in bytes, as defined by the MEG specification.
    /// </value>
    protected virtual uint MaxMegFileSize { get; } =
        MaxMegSizeProvider.GetMegMaxSize(MaxMegSizeMode.Binary).MaxFileSize;

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

        var isEncrypted = dataEntries.Any(e => e.Encrypted);
        var hasEncryptionData = fileInformation.HasEncryption;

        if (isEncrypted && !hasEncryptionData)
            return new MegFileInfoValidationResult(false, "Encryption data must be provided for encrypted MEG archives.");

        if (!isEncrypted && hasEncryptionData)
            return new MegFileInfoValidationResult(false, "No encryption data must be provided for non-encrypted MEG archives.");


        var sizeCalculator = ServiceProvider.GetRequiredService<IMegBinaryServiceFactory>()
            .GetMegSizeCalculator(fileInformation.FileVersion);

        try
        {
            foreach (var entry in dataEntries)
            {
                entry.RefreshSize();
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