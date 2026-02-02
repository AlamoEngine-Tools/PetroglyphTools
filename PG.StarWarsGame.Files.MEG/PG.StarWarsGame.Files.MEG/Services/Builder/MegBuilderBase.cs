// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using AnakinRaW.CommonUtilities;
using AnakinRaW.CommonUtilities.Extensions;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Hashing;
using PG.Commons.Utilities;
using PG.StarWarsGame.Files.MEG.Binary;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;
using PG.StarWarsGame.Files.MEG.Services.Builder.Validation;
using PG.StarWarsGame.Files.Services.Builder;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Linq;

namespace PG.StarWarsGame.Files.MEG.Services.Builder;

/// <summary>
/// Base class for a <see cref="IMegBuilder"/> service providing the fundamental implementations.
/// </summary>
public abstract class MegBuilderBase : FileBuilderBase<IReadOnlyCollection<MegDataEntryBuilderInfo>, MegFileInformation>, IMegBuilder
{
    private readonly Dictionary<Crc32, MegDataEntryBuilderInfo> _dataEntries = new();
    private readonly ICrc32HashingService _hashingService;

    internal virtual ulong MaxMegFileSize => MegFileConstants.MegMaxFileSize;

    /// <inheritdoc />
    public sealed override IReadOnlyCollection<MegDataEntryBuilderInfo> BuilderData => DataEntries;

    /// <inheritdoc/>
    [MemberNotNullWhen(true, nameof(DataEntryPathNormalizer))]
    public bool NormalizesEntryPaths => DataEntryPathNormalizer is not null;

    /// <inheritdoc/>
    public IReadOnlyCollection<MegDataEntryBuilderInfo> DataEntries => [.._dataEntries.Values];

    /// <inheritdoc/>
    /// <remarks>
    /// By default, duplicates get overwritten.
    /// </remarks>
    public virtual bool OverwritesDuplicateEntries => true;

    /// <inheritdoc/>
    /// <remarks>
    /// By default, a validator instance is used which performs specification-level checks only.
    /// </remarks>
    public virtual IMegFileInformationValidator MegFileInformationValidator { get; } = new BinaryMegFileInformationValidator();

    /// <inheritdoc/>
    /// <remarks>
    /// By default, a validator instance is used which performs no validation checks.
    /// </remarks>
    public virtual IMegDataEntryValidator DataEntryValidator { get; } = new BinaryMegEntryValidator();

    /// <inheritdoc/>
    /// <remarks>
    /// By default, no normalizer is specified.
    /// </remarks>
    public virtual IMegDataEntryPathNormalizer? DataEntryPathNormalizer => null;

    /// <summary>
    /// Initializes a new instance of the <see cref="MegBuilderBase"/> class.
    /// </summary>
    /// <param name="services">The service provider.</param>
    protected MegBuilderBase(IServiceProvider services) : base(services)
    {
        _hashingService = services.GetRequiredService<ICrc32HashingService>();
    }

    /// <inheritdoc/>
    public AddDataEntryToBuilderResult AddFile(string filePath, string entryPath, bool encrypt = false)
    {
        ThrowIfDisposed(); 
        ThrowHelper.ThrowIfNullOrEmpty(filePath);
        ThrowHelper.ThrowIfNullOrEmpty(entryPath);

        var fileInfo = FileSystem.FileInfo.New(filePath);
        if (!fileInfo.Exists)
            return AddDataEntryToBuilderResult.FromFileNotFound(fileInfo.FullName);

        return AddBuilderInfo(
            new MegDataEntryOriginInfo(fileInfo),
            entryPath,
            encrypt);
    }

    /// <inheritdoc/>
    public AddDataEntryToBuilderResult AddEntry(
        MegDataEntryLocationReference entryReference,
        string? overridePathInMeg = null,
        bool? overrideEncrypt = null)
    {
        ThrowIfDisposed();
        
        if (overridePathInMeg is not null && string.IsNullOrWhiteSpace(overridePathInMeg))
            throw new ArgumentException("Override path in MEG cannot be empty or whitespace.", nameof(overridePathInMeg));
        if (entryReference == null)
            throw new ArgumentNullException(nameof(entryReference));

        var entryPath = overridePathInMeg ?? entryReference.DataEntry.FilePath;
        var encrypt = overrideEncrypt ?? entryReference.DataEntry.Encrypted;

        if (!entryReference.Exists)
            return AddDataEntryToBuilderResult.FromEntryNotFound(entryReference);
        
        return AddBuilderInfo(
            new MegDataEntryOriginInfo(entryReference), 
            entryPath,
            encrypt);
    }

    /// <inheritdoc/>
    public bool Remove(MegDataEntryBuilderInfo info)
    {
        var crc = _hashingService.GetCrc32(info.EntryPath, MegFileConstants.MegDataEntryPathEncoding);
        return _dataEntries.Remove(crc);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        _dataEntries.Clear();
    }

    /// <inheritdoc/>
    public void BuildMany(Func<int, MegFileInformation> fileInfoFactory, bool overwrite)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public int GetMinRequiredMegFiles(MegFileVersion megVersion)
    {
        return SplitIntoMinRequiredParts(megVersion, DataEntries).Count;
    }

    private protected ICollection<MegFilePart> SplitIntoMinRequiredParts(
        MegFileVersion megVersion,
        IEnumerable<MegDataEntryBuilderInfo> builderInfo)
    {
        var metadataSizeCalculator = Services.GetRequiredService<IMegBinaryServiceFactory>()
            .GetMegSizeCalculator(megVersion);

        var parts = new List<MegFilePart>();
        var currentPart = new List<MegDataEntryBuilderInfo>();

        foreach (var entry in builderInfo)
        {
            entry.RefreshSize();
            var preCalculatedSize = metadataSizeCalculator.PreCalculateSize(entry);

            if (preCalculatedSize > MaxMegFileSize && currentPart.Count > 0)
            {
                var partSize = metadataSizeCalculator.CurrentSize;
                parts.Add(new MegFilePart(currentPart, (uint)partSize));

                currentPart = [];
                metadataSizeCalculator.Reset();
            }

            currentPart.Add(entry);
            metadataSizeCalculator.AddEntry(entry);
        }

        // Add the final part with its size
        var finalPartSize = (uint)metadataSizeCalculator.CurrentSize;
        parts.Add(new MegFilePart(currentPart, finalPartSize));
        return parts;
    }

    /// <inheritdoc />
    protected sealed override void BuildFileCore(FileSystemStream fileStream, MegFileInformation fileInformation, IReadOnlyCollection<MegDataEntryBuilderInfo> data)
    {
        var megService = Services.GetRequiredService<IMegFileService>();
        megService.CreateMegArchive(fileStream, fileInformation.FileVersion, fileInformation.EncryptionData, data);
    }

    /// <inheritdoc />
    protected sealed override bool ValidateFileInformationCore(
        MegFileInformation fileInformation, IReadOnlyCollection<MegDataEntryBuilderInfo> builderData, out string? failedReason)
    {
        if (builderData.Any(e => e.Encrypted))
            throw new NotImplementedException("Encryption is currently not supported.");

        var validation = MegFileInformationValidator.Validate(fileInformation, builderData);
        failedReason = validation.FailReason;
        return validation.IsValid;
    }

    /// <inheritdoc/>
    protected override void DisposeResources()
    {
        base.DisposeResources();
        _dataEntries.Clear();
    }
    
    private AddDataEntryToBuilderResult AddBuilderInfo(
        MegDataEntryOriginInfo originInfo,
        string entryPath,
        bool encrypt)
    {
        if (NormalizesEntryPaths)
        {
            try
            {
                entryPath = DataEntryPathNormalizer.Normalize(entryPath);
                if (string.IsNullOrEmpty(entryPath))
                    return AddDataEntryToBuilderResult.EntryNotAdded(
                        AddDataEntryToBuilderState.FailedNormalization,
                        "Normalized entry path cannot be null or empty.");
            }
            catch (Exception e)
            {
                return AddDataEntryToBuilderResult.EntryNotAdded(AddDataEntryToBuilderState.FailedNormalization,
                    e.Message);
            }
        }

        entryPath = EncodeEntryPath(entryPath, out var crc);

        if (_dataEntries.TryGetValue(crc, out var currentInfo))
        {
            if (!OverwritesDuplicateEntries)
                return AddDataEntryToBuilderResult.FromDuplicate(currentInfo.EntryPath);
        }

        MegDataEntryBuilderInfo infoToAdd;
        try
        {
            infoToAdd = new MegDataEntryBuilderInfo(originInfo, entryPath, encrypt);
        }
        catch (MegEntrySizeException)
        {
            return AddDataEntryToBuilderResult.EntryNotAdded(AddDataEntryToBuilderState.EntryFileTooLarge,
                "The entry is too large to be added to a MEG file.");
        }

        try
        {
            var validationResult = DataEntryValidator.Validate(infoToAdd);
            if (!validationResult.IsValid)
            {
                var reason = validationResult.Status switch
                {
                    MegDataEntryValidationStatus.Invalid => AddDataEntryToBuilderState.InvalidEntry,
                    MegDataEntryValidationStatus.InvalidEntryTooLarge => AddDataEntryToBuilderState.EntryFileTooLarge,
                    _ => AddDataEntryToBuilderState.InvalidEntry
                };

                var message = "The entry with entry is not valid.";
                if (validationResult.ValidationMessage is not null)
                    message += $" Reason: {validationResult.ValidationMessage}";

                return AddDataEntryToBuilderResult.EntryNotAdded(reason, message);
            }
        }
        catch (Exception e)
        {
            return AddDataEntryToBuilderResult.EntryNotAdded(AddDataEntryToBuilderState.InvalidEntry,
                $"Entry validation failed with exception: {e}");
        }
        

        _dataEntries[crc] = infoToAdd;

        return AddDataEntryToBuilderResult.EntryAdded(infoToAdd, currentInfo);
    }

    private string EncodeEntryPath(ReadOnlySpan<char> entryPath, out Crc32 crc)
    {
        var encoding = MegFileConstants.MegDataEntryPathEncoding;
        var requiredBytes = encoding.GetByteCountPG(entryPath.Length);
        var result = encoding.EncodeString(entryPath, requiredBytes);
        crc = _hashingService.GetCrc32(result, encoding);
        return result;
    }
}