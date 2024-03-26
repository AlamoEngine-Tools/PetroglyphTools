using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using AnakinRaW.CommonUtilities.Extensions;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Hashing;
using PG.Commons.Services;
using PG.Commons.Utilities;
using PG.StarWarsGame.Files.DAT.Binary;
using PG.StarWarsGame.Files.DAT.Data;
using PG.StarWarsGame.Files.DAT.Files;
using PG.StarWarsGame.Files.DAT.Services.Builder.Normalization;
using PG.StarWarsGame.Files.DAT.Services.Builder.Validation;

namespace PG.StarWarsGame.Files.DAT.Services.Builder;

/// <summary>
/// 
/// </summary>
public abstract class DatBuilderBase : ServiceBase, IDatBuilder
{
    private readonly KeyValuePairList<string, DatStringEntry> _entries = new();

    /// <inheritdoc />
    public abstract DatFileType TargetKeySortOrder { get; }

    /// <inheritdoc />
    [MemberNotNullWhen(true, nameof(KeyNormalizer))]
    public bool NormalizesKeys => KeyNormalizer is not null;

    /// <inheritdoc />
    public abstract BuilderOverrideKind KeyOverwriteBehavior { get; }

    /// <inheritdoc />
    public IReadOnlyList<DatStringEntry> DataEntries => _entries.GetValueList();

    /// <inheritdoc />
    public IReadOnlyList<DatStringEntry> SortedEntries => Crc32Utilities.SortByCrc32(_entries.GetValueList()).ToList();

    /// <inheritdoc />
    public virtual IDatKeyValidator KeyValidator => NotNullKeyValidator.Instance;

    /// <inheritdoc />
    public virtual IDatKeyNormalizer? KeyNormalizer => null;


    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    protected DatBuilderBase(IServiceProvider services) : base(services)
    {
    }

    /// <inheritdoc />
    public AddEntryResult AddEntry(string key, string value)
    {
        ThrowIfDisposed();

        if (key == null) 
            throw new ArgumentNullException(nameof(key));
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        var keyCopy = key;

        if (NormalizesKeys && !KeyNormalizer.TryNormalize(ref keyCopy, out var message))
            return AddEntryResult.EntryNotAdded(AddEntryState.FailedNormalization, message);

        if (keyCopy is null)
            throw new InvalidOperationException("key cannot be null");

        var encoding = DatFileConstants.TextKeyEncoding;
        var encodedKey = encoding.EncodeString(keyCopy, encoding.GetByteCountPG(keyCopy.Length));

        var keyValidation = KeyValidator.Validate(encodedKey);
        if (!keyValidation.IsValid)
            return AddEntryResult.EntryNotAdded(AddEntryState.InvalidKey, keyValidation.ToString());

        var crc = Services.GetRequiredService<ICrc32HashingService>().GetCrc32(encodedKey, encoding);

        var entry = new DatStringEntry(encodedKey, crc, value);

        var containsKey = _entries.ContainsKey(entry.Key, out DatStringEntry oldValue);

        if (containsKey && KeyOverwriteBehavior == BuilderOverrideKind.NoOverwrite)
            return AddEntryResult.FromDuplicate(entry);


        if (KeyOverwriteBehavior == BuilderOverrideKind.AllowDuplicate)
        {
            _entries.Add(entry.Key, entry);
            return AddEntryResult.EntryAdded(entry, containsKey);
        }

        _entries.AddOrReplace(entry.Key, entry);
        return AddEntryResult.EntryAdded(entry, oldValue);
    }

    /// <inheritdoc />
    public bool Remove(DatStringEntry entry)
    {
        return _entries.Remove(entry.Key, entry);
    }

    /// <inheritdoc />
    public bool RemoveAllKeys(string key)
    {
        return _entries.RemoveAll(key);
    }

    /// <inheritdoc />
    public void Clear()
    {
        _entries.Clear();
    }

    /// <inheritdoc />
    public void Build(DatFileInformation fileInformation, bool overwrite)
    {
        ThrowIfDisposed();

        if (fileInformation == null)
            throw new ArgumentNullException(nameof(fileInformation));

        var entries = TargetKeySortOrder == DatFileType.OrderedByCrc32 ? SortedEntries : DataEntries;

        var fileInfo = FileSystem.FileInfo.New(fileInformation.FilePath);

        // NB: This is not a full inclusive check. We leave it up to the file system to throw exceptions if something is still invalid.
        if (string.IsNullOrEmpty(fileInfo.Name) || fileInfo.Directory is null)
            throw new ArgumentException("Specified file information contains an invalid file path.", nameof(fileInformation));

        var fullPath = fileInfo.FullName;

        if (!overwrite && fileInfo.Exists)
            throw new IOException($"The file '{fullPath}' already exists.");

        fileInfo.Directory.Create();

        var megService = Services.GetRequiredService<IDatFileService>();
        using var tmpFileStream = FileSystem.File.CreateRandomHiddenTemporaryFile(fileInfo.DirectoryName);
        megService.CreateDatFile(tmpFileStream, entries, TargetKeySortOrder);
        using var destinationStream = FileSystem.FileStream.New(fullPath, FileMode.Create, FileAccess.Write);
        tmpFileStream.Seek(0, SeekOrigin.Begin);
        tmpFileStream.CopyTo(destinationStream);
    }

    /// <inheritdoc />
    public bool IsKeyValid(string key)
    {
        if (key == null) 
            throw new ArgumentNullException(nameof(key));
        return KeyValidator.Validate(key).IsValid;
    }

    /// <inheritdoc />
    protected override void DisposeManagedResources()
    {
        base.DisposeManagedResources();
        _entries.Clear();
    }
}