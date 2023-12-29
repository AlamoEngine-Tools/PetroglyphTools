using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Services;
using PG.Commons.Utilities;
using PG.StarWarsGame.Files.MEG.Binary;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Services.Builder;

/// <summary>
/// Base class for a <see cref="IMegBuilder"/> service providing the fundamental implementations.
/// </summary>
public abstract class MegBuilderBase : ServiceBase, IMegBuilder
{
    private readonly Dictionary<string, MegFileDataEntryBuilderInfo> _dataEntries = new();

    /// <inheritdoc/>
    public abstract bool NormalizesEntryPaths { get; }

    /// <inheritdoc/>
    public abstract bool EncodesEntryPaths { get; }

    /// <inheritdoc/>
    public abstract bool OverwritesDuplicateEntries { get; }

    /// <summary>
    /// Gets a value indicating whether file size information shall be retrieved when adding local file-based data entries.
    /// </summary>
    /// <remarks>
    /// By default, local files size are not determined.
    /// </remarks>
    public virtual bool AutomaticallyAddFileSizes => false;

    /// <summary>
    /// Gets the file information validator for this <see cref="IMegBuilder"/>.
    /// </summary>
    /// <remarks>
    /// By default, a validator instance is used which performs specification-level checks only.
    /// </remarks>
    protected virtual IValidator<MegBuilderFileInformationValidationData> FileInformationValidator => DefaultFileInformationValidator.Instance;

    /// <summary>
    /// Gets the data entry validator for this <see cref="IMegBuilder"/>.
    /// </summary>
    /// <remarks>
    /// By default, a validator instance is used which performs no validation checks.
    /// </remarks>
    protected virtual IValidator<MegFileDataEntryBuilderInfo> DataEntryValidator => AlwaysValidDataEntryValidator.Instance;

    /// <inheritdoc/>
    public IReadOnlyCollection<MegFileDataEntryBuilderInfo> DataEntries => new List<MegFileDataEntryBuilderInfo>(_dataEntries.Values);

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

        if (encrypt)
        {
            throw new NotImplementedException("Encryption is currently not supported.");
        }

        var fileInfo = FileSystem.FileInfo.New(filePath);
        if (!FileSystem.File.Exists(filePath))
            return AddDataEntryToBuilderResult.FromFileNotFound(fileInfo.FullName);

        long? fileSize = AutomaticallyAddFileSizes ? fileInfo.Length : null;
        if (fileSize > uint.MaxValue)
        {
            return AddDataEntryToBuilderResult.EntryNotAdded(AddDataEntryToBuilderState.InvalidEntry,
                $"Source file '{fileInfo.FullName}' larger than 4GB.");
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

        if (encrypt)
        {
            throw new NotImplementedException("Encryption is currently not supported.");
        }

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

        if (fileInformation == null)
            throw new ArgumentNullException(nameof(fileInformation));

        if (fileInformation.HasEncryption)
        {
            throw new NotImplementedException("Encryption is currently not supported.");
        }

        // Prevent races by creating getting a copy of the current state
        var dataEntries = DataEntries;

        var validationResult = FileInformationValidator.Validate(new(fileInformation, dataEntries));
        if (!validationResult.IsValid)
            throw new NotSupportedException($"Provided file parameters are not valid for this builder: {validationResult}");

        var fileInfo = FileSystem.FileInfo.New(fileInformation.FilePath);

        // file path points to a directory
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
        return FileInformationValidator.Validate(new(fileInformation, DataEntries)).IsValid;
    }

    /// <summary>
    /// When overridden, normalizes specified file path string.
    /// </summary>
    /// <remarks>
    /// The default implementation does not normalize the path and leaves <paramref name="filePath"/> unchanged.
    /// </remarks>
    /// <param name="filePath">The file path to normalize.</param>
    /// <param name="message">Optional error message if normalization failed or <paramref name="filePath"/> is not supported.</param>
    /// <returns><see langword="true"/> if <paramref name="filePath"/> was successfully normalized; otherwise, <see langword="false"/>.</returns>
    protected virtual bool NormalizePath(ref string filePath, out string? message)
    {
        message = null;
        return true;
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

        if (!NormalizePath(ref actualFilePath, out var message))
        {
            return AddDataEntryToBuilderResult.EntryNotAdded(AddDataEntryToBuilderState.FailedNormalization, message);
        }

        Commons.Utilities.ThrowHelper.ThrowIfNullOrEmpty(actualFilePath);

        if (EncodesEntryPaths)
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


    /// <summary>
    /// Validates a specified <see cref="MegFileInformation"/> is compliant to the MEG specification.
    /// </summary>
    protected internal class DefaultFileInformationValidator : AbstractValidator<MegBuilderFileInformationValidationData>
    {
        /// <summary>
        /// Gets a singleton instance of the <see cref="DefaultFileInformationValidator"/> class.
        /// </summary>
        public static readonly DefaultFileInformationValidator Instance = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultFileInformationValidator"/> class with rules for ensuring MEG specification
        /// compliant <see cref="MegFileInformation"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="MegFileInformation"/> are considered <b>not</b> to be compliant if the MEG is encrypted but the version is not V3.
        /// </remarks>
        public DefaultFileInformationValidator()
        {
            RuleFor(x => x.FileInformation).NotNull();
            RuleFor(x => x.DataEntries).NotNull();
            RuleFor(x => x.FileInformation.FileVersion)
                .Must(v => v == MegFileVersion.V3)
                .When(x => x.DataEntries.Any(d => d.Encrypted));
        }
    }

    /// <summary>
    /// This class always passes the validation of a <see cref="MegFileDataEntryBuilderInfo"/>.
    /// </summary>
    protected internal class AlwaysValidDataEntryValidator : AbstractValidator<MegFileDataEntryBuilderInfo>
    {
        /// <summary>
        /// Gets a singleton instance of the <see cref="AlwaysValidDataEntryValidator"/> class.
        /// </summary>
        public static readonly AlwaysValidDataEntryValidator Instance = new();
    }
}