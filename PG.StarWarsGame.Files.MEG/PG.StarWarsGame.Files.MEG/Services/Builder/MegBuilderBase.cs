using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Services;
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
public abstract class MegBuilderBase : ServiceBase, IMegBuilder
{
    private readonly Dictionary<string, MegFileDataEntryBuilderInfo> _dataEntries = new();

    /// <inheritdoc/>
    [MemberNotNullWhen(true, nameof(DataEntryPathNormalizer))]
    public bool NormalizesEntryPaths => DataEntryPathNormalizer is not null;

    /// <inheritdoc/>
    public IReadOnlyCollection<MegFileDataEntryBuilderInfo> DataEntries => new List<MegFileDataEntryBuilderInfo>(_dataEntries.Values);

    /// <remarks>
    /// By default, duplicates get overwritten.
    /// </remarks>
    /// <inheritdoc/>
    public virtual bool OverwritesDuplicateEntries => true;

    /// <summary>
    /// Gets a value indicating whether file size information shall be retrieved when adding local file-based data entries.
    /// </summary>
    /// <remarks>
    /// By default, local files size are not determined.
    /// </remarks>
    public virtual bool AutomaticallyAddFileSizes => false;

    /// <remarks>
    /// By default, a validator instance is used which performs specification-level checks only.
    /// </remarks>
    /// <inheritdoc/>
    public virtual IMegFileInformationValidator MegFileInformationValidator => DefaultMegFileInformationValidator.Instance;

    /// <remarks>
    /// By default, a validator instance is used which performs no validation checks.
    /// </remarks>
    /// <inheritdoc/>
    public virtual IBuilderInfoValidator DataEntryValidator => NotNullDataEntryValidator.Instance;

    /// <remarks>
    /// By default, no normalizer is specified.
    /// </remarks>
    /// <inheritdoc/>
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

        Commons.Utilities.ThrowHelper.ThrowIfNullOrEmpty(filePath);
        Commons.Utilities.ThrowHelper.ThrowIfNullOrEmpty(filePathInMeg);

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
        Commons.Utilities.ThrowHelper.ThrowIfNullOrEmpty(filePath);

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

    /// <inheritdoc/>
    /// <remarks>
    /// <paramref name="fileInformation"/> may specify relative or absolute path information.
    /// Relative path information is interpreted as relative to the current working directory.
    /// <br/>
    /// <br/>
    /// Any and all directories specified in <paramref name="fileInformation"/> are created, unless they already exist or unless some part of path is invalid.
    /// </remarks>
    public void Build(MegFileInformation fileInformation, bool overwrite)
    {
        ThrowIfDisposed();

        if (fileInformation is null)
            throw new ArgumentNullException(nameof(fileInformation));

        // Prevent races by creating getting a copy of the current state
        var dataEntries = DataEntries;

        if (dataEntries.Any(e => e.Encrypted))
        {
            throw new NotImplementedException("Encryption is currently not supported.");
        }

        var validationResult = MegFileInformationValidator.Validate(new(fileInformation, dataEntries));
        if (!validationResult.IsValid)
            throw new NotSupportedException($"Provided file parameters are not valid for this builder: {validationResult}");

        var fileInfo = FileSystem.FileInfo.New(fileInformation.FilePath);

        // file path points to a directory
        // NB: This is not a full inclusive check. We leave it up to the file system to throw exceptions if something is still invalid.
        if (string.IsNullOrEmpty(fileInfo.Name) || fileInfo.Directory is null)
            throw new ArgumentException("Specified file information contains an invalid file path.", nameof(fileInformation));

        var fullPath = fileInfo.FullName;

        if (!overwrite && fileInfo.Exists)
            throw new IOException($"The file '{fullPath}' already exists.");

        fileInfo.Directory.Create();

        // TODO: Create hidden temp file properly
        var tempFile = fullPath + ".tmp";

        var megService = Services.GetRequiredService<IMegFileService>();
        try
        {
            using (var fileStream = FileSystem.FileStream.New(tempFile, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                megService.CreateMegArchive(fileStream, fileInformation.FileVersion, fileInformation.EncryptionData, dataEntries);
            }
            // TODO: Move with overwrite
            FileSystem.File.Move(tempFile, fullPath);
        }
        finally
        {
            try
            {
                FileSystem.File.Delete(tempFile);
            }
            catch
            {
                // Ignore
            }
        }
    }

    /// <summary>
    /// Checks whether the passed file information are valid for this <see cref="IMegBuilder"/>.
    /// </summary>
    /// <remarks>
    /// The default implementation does not validate and always returns <see langword="true"/>.
    /// </remarks>
    /// <param name="fileInformation">The file information to validate</param>
    /// <returns><see langword="true"/> if the passed file information are valid; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="fileInformation"/> is <see langword="null"/>.</exception>
    public bool ValidateFileInformation(MegFileInformation fileInformation)
    {
        if (fileInformation == null)
            throw new ArgumentNullException(nameof(fileInformation));
        return MegFileInformationValidator.Validate(new(fileInformation, DataEntries)).IsValid;
    }

    /// <inheritdoc/>
    protected override void DisposeManagedResources()
    {
        base.DisposeManagedResources();
        _dataEntries.Clear();
    }

    private AddDataEntryToBuilderResult AddBuilderInfo(string filePath, Func<string, MegFileDataEntryBuilderInfo> createBuilderInfo)
    {
        Commons.Utilities.ThrowHelper.ThrowIfNullOrEmpty(filePath);

        var actualFilePath = filePath;

        if (NormalizesEntryPaths && !DataEntryPathNormalizer.TryNormalizePath(ref actualFilePath, out var message))
            return AddDataEntryToBuilderResult.EntryNotAdded(AddDataEntryToBuilderState.FailedNormalization, message);

        Commons.Utilities.ThrowHelper.ThrowIfNullOrEmpty(actualFilePath);

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