// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using AnakinRaW.CommonUtilities;
using AnakinRaW.CommonUtilities.Extensions;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Hashing;
using PG.Commons.Utilities;
using PG.StarWarsGame.Files.MEG.Binary;
using PG.StarWarsGame.Files.MEG.Binary.Size;
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
public abstract class MegBuilderBase
    : FileBuilderBase<IReadOnlyCollection<MegDataEntryBuilderInfo>, MegFileInformation>, IMegBuilder
{
    private readonly Dictionary<Crc32, MegDataEntryBuilderInfo> _dataEntryTable = new();
    private readonly ICrc32HashingService _hashingService;
    
    /// <inheritdoc />
    public sealed override IReadOnlyCollection<MegDataEntryBuilderInfo> BuilderData => DataEntries;

    /// <inheritdoc/>
    [MemberNotNullWhen(true, nameof(DataEntryPathNormalizer))]
    public bool NormalizesEntryPaths => DataEntryPathNormalizer is not null;

    /// <inheritdoc/>
    public IReadOnlyCollection<MegDataEntryBuilderInfo> DataEntries { get; }

    /// <inheritdoc/>
    /// <value>
    /// <see langword="true"/> by default.
    /// </value>
    public virtual bool OverwritesDuplicateEntries => true;

    /// <inheritdoc/>
    /// <value>
    /// <seealso cref="BinaryMegFileInformationValidator"/> by default.
    /// </value>
    public virtual IMegFileInformationValidator MegFileInformationValidator { get; }

    /// <inheritdoc/>
    /// <value>
    /// <seealso cref="BinaryMegDataEntryValidator"/> by default.
    /// </value>
    public virtual IMegDataEntryValidator DataEntryValidator { get; } = new BinaryMegDataEntryValidator();

    /// <inheritdoc/>
    /// <value>
    /// <see langword="null"/>, meaning no normalizer is specified.
    /// </value>
    public virtual IMegDataEntryPathNormalizer? DataEntryPathNormalizer => null;

    /// <summary>
    /// Gets the maximum allowed size, in bytes, for a MEG file created by this builder.
    /// </summary>
    /// <value>
    /// 4GB (2^32 - 1 bytes) by default 
    /// </value>
    public virtual uint MaxMegFileSize { get; } = MaxMegSizeProvider.GetMegMaxSize(MaxMegSizeMode.Binary).MaxFileSize;

    /// <summary>
    /// Initializes a new instance of the <see cref="MegBuilderBase"/> class.
    /// </summary>
    /// <param name="services">The service provider.</param>
    protected MegBuilderBase(IServiceProvider services) : base(services)
    {
        _hashingService = services.GetRequiredService<ICrc32HashingService>();
        MegFileInformationValidator = new BinaryMegFileInformationValidator(services);
        DataEntries = _dataEntryTable.Values;
    }

    /// <inheritdoc/>
    public MegDataEntryAddResult AddFile(string filePath, string entryPath, bool encrypt = false)
    {
        ThrowIfDisposed(); 
        ThrowHelper.ThrowIfNullOrEmpty(filePath);
        ThrowHelper.ThrowIfNullOrEmpty(entryPath);

        var fileInfo = FileSystem.FileInfo.New(filePath);
        if (!fileInfo.Exists)
            return MegDataEntryAddResult.FromFileNotFound(fileInfo.FullName);

        return AddBuilderInfo(
            new MegDataEntryOriginInfo(fileInfo),
            entryPath,
            encrypt);
    }

    /// <inheritdoc/>
    public MegDataEntryAddResult AddEntry(
        MegDataEntryLocationReference entryReference,
        string? overridePathInMeg = null,
        bool? overrideEncrypt = null)
    {
        ThrowIfDisposed();
        
        if (overridePathInMeg is not null && string.IsNullOrWhiteSpace(overridePathInMeg))
            throw new ArgumentException("Override path in MEG cannot be empty or whitespace.", nameof(overridePathInMeg));
        if (entryReference == null)
            throw new ArgumentNullException(nameof(entryReference));

        var entryPath = overridePathInMeg ?? entryReference.DataEntry.Path;
        var encrypt = overrideEncrypt ?? entryReference.DataEntry.Encrypted;

        if (!entryReference.Exists)
            return MegDataEntryAddResult.FromEntryNotFound(entryReference);
        
        return AddBuilderInfo(
            new MegDataEntryOriginInfo(entryReference), 
            entryPath,
            encrypt);
    }

    /// <inheritdoc/>
    public bool Remove(MegDataEntryBuilderInfo info)
    {
        var crc = _hashingService.GetCrc32(info.EntryPath, MegFileConstants.MegDataEntryPathEncoding);
        return _dataEntryTable.Remove(crc);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        _dataEntryTable.Clear();
    }

    /// <inheritdoc/>
    public void BuildMany(MegFileInformation initialFileInformation, Func<int, string> filePathFactory, bool overwrite)
    {
        if (initialFileInformation == null) 
            throw new ArgumentNullException(nameof(initialFileInformation));
        if (filePathFactory == null) 
            throw new ArgumentNullException(nameof(filePathFactory));

        var megParts = SplitIntoMinRequiredParts(initialFileInformation.FileVersion, DataEntries);

        if (megParts.Count == 1)
        {
            Build(initialFileInformation, overwrite);
            return;
        }
        
        var count = 1;
        foreach (var megPart in megParts)
        {
            var fileInfo = initialFileInformation with
            {
                FilePath = filePathFactory(count)
            };

            using var delegatingBuilder = new DelegatingMegBuilder(this);
            delegatingBuilder.AddEntriesUnsafe(megPart);
            
            delegatingBuilder.Build(fileInfo, overwrite);
            count++;
        }
    }

    private void AddEntriesUnsafe(ICollection<MegDataEntryBuilderInfo> entries)
    {
        foreach (var entry in entries)
        {
            var crc = _hashingService.GetCrc32(entry.EntryPath, MegFileConstants.MegDataEntryPathEncoding);
            _dataEntryTable[crc] = entry;
        }
    }
    
    /// <inheritdoc/>
    public int GetMinRequiredMegFiles(MegFileVersion megVersion)
    {
        return SplitIntoMinRequiredParts(megVersion, DataEntries).Count;
    }

    /// <summary>
    /// Splits the specified collection of <see cref="MegDataEntryBuilderInfo"/> into the minimum number of parts
    /// required to fit within the maximum allowed size for a MEG file, based on the specified MEG file version.
    /// </summary>
    /// <param name="megVersion">The version of the MEG file, which determines the size constraints and metadata calculations.</param>
    /// <param name="builderInfo">The collection of <see cref="MegDataEntryBuilderInfo"/> objects to be split into parts.</param>
    /// <returns>
    /// A collection of collections, where each inner collection represents a part containing 
    /// <see cref="MegDataEntryBuilderInfo"/> objects that fit within the size constraints.
    /// </returns>
    /// <remarks>
    /// This method ensures that the entries are grouped into parts such that each part adheres to the 
    /// maximum file size limit defined by the MEG file version. If an individual entry exceeds the 
    /// maximum size, it will be placed in its own part.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// It is impossible to split into multiple MEG files because of the current state of the builder.
    /// </exception>
    protected ICollection<ICollection<MegDataEntryBuilderInfo>> SplitIntoMinRequiredParts(
        MegFileVersion megVersion,
        IEnumerable<MegDataEntryBuilderInfo> builderInfo)
    {
        var metadataSizeCalculator = Services.GetRequiredService<IMegBinaryServiceFactory>()
            .GetMegSizeCalculator(megVersion);

        var parts = new List<ICollection<MegDataEntryBuilderInfo>>();
        var currentPart = new List<MegDataEntryBuilderInfo>();

        var maxFileSize = MaxMegFileSize;
        
        foreach (var entry in builderInfo)
        {
            var preCalculatedSize = metadataSizeCalculator.PreCalculateSize(entry);

            if (preCalculatedSize > maxFileSize)
            {
                if (currentPart.Count > 0)
                {
                    parts.Add(currentPart);
                    currentPart = [];
                    metadataSizeCalculator.Reset();
                    preCalculatedSize = metadataSizeCalculator.PreCalculateSize(entry);
                }

                if (preCalculatedSize > maxFileSize)
                    throw new InvalidOperationException("Unable to build MEG archive from current builder entries.");
            }

            currentPart.Add(entry);
            metadataSizeCalculator.AddEntry(entry);
        }
        parts.Add(currentPart);
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
        _dataEntryTable.Clear();
    }
    
    private MegDataEntryAddResult AddBuilderInfo(
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
                    return MegDataEntryAddResult.EntryNotAdded(
                        MegDataEntryAddStatus.FailedNormalization,
                        "Normalized entry path cannot be null or empty.");
            }
            catch (Exception e)
            {
                return MegDataEntryAddResult.EntryNotAdded(MegDataEntryAddStatus.FailedNormalization,
                    e.Message);
            }
        }

        entryPath = EncodeEntryPath(entryPath, out var crc);

        if (_dataEntryTable.TryGetValue(crc, out var currentInfo))
        {
            if (!OverwritesDuplicateEntries)
                return MegDataEntryAddResult.FromDuplicate(currentInfo.EntryPath);
        }

        MegDataEntryBuilderInfo infoToAdd;
        try
        {
            infoToAdd = new MegDataEntryBuilderInfo(originInfo, entryPath, encrypt);

        }
        catch (MegEntrySizeException)
        {
            return MegDataEntryAddResult.EntryNotAdded(MegDataEntryAddStatus.EntryFileTooLarge,
                "The entry is too large to be added to a MEG file.");
        }

        try
        {
            var validationResult = DataEntryValidator.Validate(infoToAdd);
            if (!validationResult.IsValid)
            {
                var reason = validationResult.Status switch
                {
                    MegDataEntryValidationStatus.Invalid => MegDataEntryAddStatus.InvalidEntry,
                    MegDataEntryValidationStatus.InvalidEntryTooLarge => MegDataEntryAddStatus.EntryFileTooLarge,
                    _ => MegDataEntryAddStatus.InvalidEntry
                };

                var message = "The entry with entry is not valid.";
                if (validationResult.ValidationMessage is not null)
                    message += $" Reason: {validationResult.ValidationMessage}";

                return MegDataEntryAddResult.EntryNotAdded(reason, message);
            }
        }
        catch (Exception e)
        {
            return MegDataEntryAddResult.EntryNotAdded(MegDataEntryAddStatus.InvalidEntry,
                $"Entry validation failed with exception: {e}");
        }
        

        _dataEntryTable[crc] = infoToAdd;

        return MegDataEntryAddResult.EntryAdded(infoToAdd, currentInfo);
    }

    private string EncodeEntryPath(ReadOnlySpan<char> entryPath, out Crc32 crc)
    {
        var encoding = MegFileConstants.MegDataEntryPathEncoding;
        var requiredBytes = encoding.GetByteCountPG(entryPath.Length);
        var result = encoding.EncodeString(entryPath, requiredBytes);
        crc = _hashingService.GetCrc32(result, encoding);
        return result;
    }

    private class DelegatingMegBuilder(MegBuilderBase megBuilderBase) : MegBuilderBase(megBuilderBase.Services)
    {
        public override IMegDataEntryPathNormalizer? DataEntryPathNormalizer => megBuilderBase.DataEntryPathNormalizer;

        public override IMegDataEntryValidator DataEntryValidator => megBuilderBase.DataEntryValidator;

        public override bool OverwritesDuplicateEntries => megBuilderBase.OverwritesDuplicateEntries;

        public override IMegFileInformationValidator MegFileInformationValidator =>
            megBuilderBase.MegFileInformationValidator;

        public override uint MaxMegFileSize => megBuilderBase.MaxMegFileSize;
    }
}