using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Linq;
using AnakinRaW.CommonUtilities.Extensions;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Utilities;
using PG.StarWarsGame.Files.MEG.Binary;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;
using PG.StarWarsGame.Files.MEG.Services.Builder.Validation;
using AnakinRaW.CommonUtilities;
using PG.Commons.Services.Builder;

namespace PG.StarWarsGame.Files.MEG.Services.Builder;

/// <summary>
/// Base class for a <see cref="IMegBuilder"/> service providing the fundamental implementations.
/// </summary>
public abstract class MegBuilderBase : FileBuilderBase<IReadOnlyCollection<MegFileDataEntryBuilderInfo>, MegFileInformation>, IMegBuilder
{
    private readonly Dictionary<string, MegFileDataEntryBuilderInfo> _dataEntries = new();

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
    public virtual IBuilderInfoValidator DataEntryValidator => NotNullDataEntryValidator.Instance;

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
    }

    /// <inheritdoc/>
    public AddDataEntryToBuilderResult AddFile(string filePath, string filePathInMeg, bool encrypt = false)
    {
        ThrowIfDisposed();

        ThrowHelper.ThrowIfNullOrEmpty(filePath);
        ThrowHelper.ThrowIfNullOrEmpty(filePathInMeg);

        var fileInfo = FileSystem.FileInfo.New(filePath);
        if (!fileInfo.Exists)
            return AddDataEntryToBuilderResult.FromFileNotFound(fileInfo.FullName);

        long? fileSize = AutomaticallyAddFileSizes ? fileInfo.Length : null;
        if (fileSize > uint.MaxValue)
        {
            return AddDataEntryToBuilderResult.EntryNotAdded(AddDataEntryToBuilderState.EntryFileTooLarge,
                $"Source file '{fileInfo.FullName}' is larger than 4GB.");
        }

        return AddBuilderInfo(filePathInMeg,
            actualFilePath =>
                MegFileDataEntryBuilderInfo.FromFile(fileInfo.FullName, actualFilePath, (uint?)fileSize, encrypt));
    }

    /// <inheritdoc/>
    public AddDataEntryToBuilderResult AddEntry(
        MegDataEntryLocationReference entryReference,
        string? overridePathInMeg = null,
        bool? overrideEncrypt = null)
    {
        ThrowIfDisposed();

        if (entryReference == null)
            throw new ArgumentNullException(nameof(entryReference));

        var filePath = overridePathInMeg ?? entryReference.DataEntry.FilePath;
        ThrowHelper.ThrowIfNullOrEmpty(filePath);

        var encrypt = overrideEncrypt ?? entryReference.DataEntry.Encrypted;

        if (!entryReference.Exists)
            return AddDataEntryToBuilderResult.FromEntryNotFound(entryReference);

        return AddBuilderInfo(filePath,
            actualFilePath => MegFileDataEntryBuilderInfo.FromEntryReference(entryReference, actualFilePath, encrypt));
    }

    /// <inheritdoc/>
    public bool Remove(MegFileDataEntryBuilderInfo info)
    {
        return _dataEntries.Remove(info.FilePath);
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

    private AddDataEntryToBuilderResult AddBuilderInfo(string filePath, Func<string, MegFileDataEntryBuilderInfo> createBuilderInfo)
    {
        ThrowHelper.ThrowIfNullOrEmpty(filePath);

        var actualFilePath = filePath;

        if (NormalizesEntryPaths && !DataEntryPathNormalizer.TryNormalize(ref actualFilePath, out var message))
            return AddDataEntryToBuilderResult.EntryNotAdded(AddDataEntryToBuilderState.FailedNormalization, message);

        if (string.IsNullOrEmpty(actualFilePath))
            throw new InvalidOperationException("filePath cannot be null");

        actualFilePath = EncodePath(actualFilePath);

        if (_dataEntries.TryGetValue(actualFilePath, out var currentInfo))
        {
            if (!OverwritesDuplicateEntries)
                return AddDataEntryToBuilderResult.FromDuplicate(actualFilePath);
        }

        var infoToAdd = createBuilderInfo(actualFilePath);

        var entryValidation = DataEntryValidator.Validate(infoToAdd);
        if (!entryValidation.IsValid)
            return AddDataEntryToBuilderResult.EntryNotAdded(AddDataEntryToBuilderState.InvalidEntry, entryValidation.ToString());

        _dataEntries[actualFilePath] = infoToAdd;

        return AddDataEntryToBuilderResult.EntryAdded(infoToAdd, currentInfo);
    }

    private static string EncodePath(string actualFilePath)
    {
        var encoding = MegFileConstants.MegDataEntryPathEncoding;
        return encoding.EncodeString(actualFilePath, encoding.GetByteCountPG(actualFilePath.Length));
    }
}