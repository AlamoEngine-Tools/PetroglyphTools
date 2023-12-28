using System;
using System.Collections.Generic;
using PG.Commons.Services;
using PG.Commons.Utilities;
using PG.StarWarsGame.Files.MEG.Binary;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Services.Builders;

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
    /// 
    /// </summary>
    protected virtual bool AutomaticallyAddFileSizes => false;

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
    public void Build(MegFileInformation fileParams, bool overwrite)
    {
        if (!AreFileParamsValid(fileParams))
            throw new NotSupportedException("The passed file information are not valid or supported by this builder.");


        throw new NotImplementedException();
    }

    /// <summary>
    /// When overridden, checks whether the passed file information are valid for this <see cref="IMegBuilder"/>.
    /// </summary>
    /// <remarks>
    /// The default implementation does not validate and always returns <see langword="true"/>.
    /// </remarks>
    /// <param name="fileParams">The file information to validate</param>
    /// <returns><see langword="true"/> if the passed file information are valid; otherwise, <see langword="false"/>.</returns>
    public virtual bool AreFileParamsValid(MegFileInformation fileParams)
    {
        return true;
    }

    /// <summary>
    /// Gets called when a builder info is about to get added to this <see cref="IMegBuilder"/>.
    /// When overridden allows to validate the builder info and reject adding it.
    /// </summary>
    /// <param name="infoToAdd">The info that is about to get added.</param>
    /// <param name="notAddedMessage">An optional message that is returned when <paramref name="infoToAdd"/> shall not be added.</param>
    /// <returns><see langword="true"/> if the builder info can be added; otherwise, <see langword="false"/>.</returns>
    protected bool OnAdding(MegFileDataEntryBuilderInfo infoToAdd, out string? notAddedMessage)
    {
        notAddedMessage = string.Empty;
        return true;
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

        if (!OnAdding(infoToAdd, out var reasonMessage))
            return AddDataEntryToBuilderResult.EntryNotAdded(AddDataEntryToBuilderState.InvalidEntry, reasonMessage);

        _dataEntries[actualFilePath] = infoToAdd;

        return AddDataEntryToBuilderResult.EntryAdded(infoToAdd, currentInfo);
    }

    private static string EncodePath(string actualFilePath)
    {
        var encoding = MegFileConstants.MegDataEntryPathEncoding;
        return encoding.EncodeString(actualFilePath, encoding.GetByteCountPG(actualFilePath.Length));
    }
}