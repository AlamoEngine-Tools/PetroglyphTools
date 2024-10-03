// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Linq;
using AnakinRaW.CommonUtilities;
using AnakinRaW.CommonUtilities.Extensions;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Hashing;
using PG.Commons.Services.Builder;
using PG.Commons.Utilities;
using PG.StarWarsGame.Files.MEG.Binary;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;
using PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

namespace PG.StarWarsGame.Files.MEG.Services.Builder;

/// <summary>
/// Base class for a <see cref="IMegBuilder"/> service providing the fundamental implementations.
/// </summary>
public abstract class MegBuilderBase : FileBuilderBase<IReadOnlyCollection<MegFileDataEntryBuilderInfo>, MegFileInformation>, IMegBuilder
{
    private readonly Dictionary<Crc32, MegFileDataEntryBuilderInfo> _dataEntries = new();
    private readonly ICrc32HashingService _hashingService;

    /// <inheritdoc />
    public sealed override IReadOnlyCollection<MegFileDataEntryBuilderInfo> BuilderData => DataEntries;

    /// <inheritdoc/>
    [MemberNotNullWhen(true, nameof(DataEntryPathNormalizer))]
    public bool NormalizesEntryPaths => DataEntryPathNormalizer is not null;

    /// <inheritdoc/>
    public IReadOnlyCollection<MegFileDataEntryBuilderInfo> DataEntries => new List<MegFileDataEntryBuilderInfo>(_dataEntries.Values);

    /// <inheritdoc/>
    /// <remarks>
    /// By default, duplicates get overwritten.
    /// </remarks>
    public virtual bool OverwritesDuplicateEntries => true;

    /// <summary>
    /// Gets a value indicating whether file size information shall be retrieved when adding local file-based data entries.
    /// </summary>
    /// <remarks>
    /// By default, local files size are not determined.
    /// </remarks>
    public virtual bool AutomaticallyAddFileSizes => false;

    /// <inheritdoc/>
    /// <remarks>
    /// By default, a validator instance is used which performs specification-level checks only.
    /// </remarks>
    public virtual IMegFileInformationValidator MegFileInformationValidator =>
        DefaultMegFileInformationValidator.Instance;

    /// <inheritdoc/>
    /// <remarks>
    /// By default, a validator instance is used which performs no validation checks.
    /// </remarks>
    public virtual IMegBuilderInfoValidator DataEntryValidator => NotNullDataEntryValidator.Instance;

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
    public unsafe AddDataEntryToBuilderResult AddFile(string filePath, string entryPath, bool encrypt = false)
    {
        ThrowIfDisposed();

        ThrowHelper.ThrowIfNullOrEmpty(filePath);
        ThrowHelper.ThrowIfNullOrEmpty(entryPath);

        var fileInfo = FileSystem.FileInfo.New(filePath);
        if (!fileInfo.Exists)
            return AddDataEntryToBuilderResult.FromFileNotFound(fileInfo.FullName);

        long? fileSize = AutomaticallyAddFileSizes ? fileInfo.Length : null;
        if (fileSize > uint.MaxValue)
        {
            return AddDataEntryToBuilderResult.EntryNotAdded(AddDataEntryToBuilderState.EntryFileTooLarge,
                $"Source file '{fileInfo.FullName}' is larger than 4GB.");
        }

        return AddBuilderInfo(
            entryPath.AsSpan(),
            (uint?)fileSize,
            encrypt,
            &OriginInfoFromFile, 
            fileInfo.FullName);
    }

    /// <inheritdoc/>
    public unsafe AddDataEntryToBuilderResult AddEntry(
        MegDataEntryLocationReference entryReference,
        string? overridePathInMeg = null,
        bool? overrideEncrypt = null)
    {
        ThrowIfDisposed();

        if (entryReference == null)
            throw new ArgumentNullException(nameof(entryReference));

        var entryPath = overridePathInMeg ?? entryReference.DataEntry.FilePath;
        ThrowHelper.ThrowIfNullOrEmpty(entryPath);

        var encrypt = overrideEncrypt ?? entryReference.DataEntry.Encrypted;

        if (!entryReference.Exists)
            return AddDataEntryToBuilderResult.FromEntryNotFound(entryReference);

        return AddBuilderInfo(
            entryPath.AsSpan(), 
            entryReference.DataEntry.Location.Size,
            encrypt,
            &OriginInfoFromLocation, 
            entryReference);
    }

    private static MegDataEntryOriginInfo OriginInfoFromFile(string filePath)
    {
        return new MegDataEntryOriginInfo(filePath);
    }

    private static MegDataEntryOriginInfo OriginInfoFromLocation(MegDataEntryLocationReference location)
    {
        return new MegDataEntryOriginInfo(location);
    }

    /// <inheritdoc/>
    public bool Remove(MegFileDataEntryBuilderInfo info)
    {
        var crc = _hashingService.GetCrc32(info.FilePath, MegFileConstants.MegDataEntryPathEncoding);
        return _dataEntries.Remove(crc);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        _dataEntries.Clear();
    }

    /// <inheritdoc />
    protected sealed override void BuildFileCore(FileSystemStream fileStream, MegFileInformation fileInformation, IReadOnlyCollection<MegFileDataEntryBuilderInfo> data)
    {
        var megService = Services.GetRequiredService<IMegFileService>();
        megService.CreateMegArchive(fileStream, fileInformation.FileVersion, fileInformation.EncryptionData, data);
    }

    /// <inheritdoc />
    protected sealed override bool ValidateFileInformationCore(MegFileInformation fileInformation, IReadOnlyCollection<MegFileDataEntryBuilderInfo> builderData,
        out string? failedReason)
    {
        if (builderData.Any(e => e.Encrypted))
            throw new NotImplementedException("Encryption is currently not supported.");

        var validation = MegFileInformationValidator.Validate(new(fileInformation, DataEntries));
        failedReason = validation.ToString();
        return validation.IsValid;
    }

    /// <inheritdoc/>
    protected override void DisposeManagedResources()
    {
        base.DisposeManagedResources();
        _dataEntries.Clear();
    }
    
    private unsafe AddDataEntryToBuilderResult AddBuilderInfo<T>(
        ReadOnlySpan<char> entryPath, 
        uint? size,
        bool encrypt,
        delegate*<T, MegDataEntryOriginInfo> originInfoFactory,
        T state)
    {
        if (entryPath.Length == 0)
            throw new ArgumentException("entryPath cannot be empty", nameof(entryPath));


        var entryPathStringBuilder = new ValueStringBuilder(stackalloc char[260]);
        
        try
        {
            if (NormalizesEntryPaths)
            {
                try
                {
                    DataEntryPathNormalizer.Normalize(entryPath, ref entryPathStringBuilder);
                }
                catch (Exception e)
                {
                    return AddDataEntryToBuilderResult.EntryNotAdded(AddDataEntryToBuilderState.FailedNormalization, e.Message);
                }
            }
            else
                entryPathStringBuilder.Append(entryPath);


            if (entryPathStringBuilder.Length == 0)
                throw new InvalidOperationException("entryPath cannot be null");

            var entryLength = entryPathStringBuilder.Length;
            var encodedEntryBuffer = entryLength > 265 ? new char[entryLength] : stackalloc char[entryLength];
            var encodedEntry = EncodeEntryPath(entryPathStringBuilder.AsSpan(), encodedEntryBuffer, out Crc32 crc);

            var validationResult = DataEntryValidator.Validate(encodedEntry, encrypt, size);
            if (!validationResult)
                return AddDataEntryToBuilderResult.EntryNotAdded(AddDataEntryToBuilderState.InvalidEntry,
                    $"The entry with entry path '{encodedEntry.ToString()}' is not valid.");

            if (_dataEntries.TryGetValue(crc, out var currentInfo))
            {
                if (!OverwritesDuplicateEntries)
                    return AddDataEntryToBuilderResult.FromDuplicate(currentInfo.FilePath);
            }

            var infoToAdd = new MegFileDataEntryBuilderInfo(originInfoFactory(state), encodedEntry.ToString(), size, encrypt);

            _dataEntries[crc] = infoToAdd;

            return AddDataEntryToBuilderResult.EntryAdded(infoToAdd, currentInfo);
        }
        finally
        {
            entryPathStringBuilder.Dispose();
        }
    }

    private ReadOnlySpan<char> EncodeEntryPath(ReadOnlySpan<char> entryPath, Span<char> buffer, out Crc32 crc)
    {
        var encoding = MegFileConstants.MegDataEntryPathEncoding;
        var requiredBytes = encoding.GetByteCountPG(entryPath.Length);
        var length = encoding.EncodeString(entryPath, buffer, requiredBytes);
        var result = buffer.Slice(0, length);
        crc = _hashingService.GetCrc32(result, encoding);
        return result;
    }
}