// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using AnakinRaW.CommonUtilities.Extensions;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Hashing;
using PG.Commons.Services.Builder;
using PG.Commons.Utilities;
using PG.StarWarsGame.Files.DAT.Binary;
using PG.StarWarsGame.Files.DAT.Data;
using PG.StarWarsGame.Files.DAT.Files;
using PG.StarWarsGame.Files.DAT.Services.Builder.Validation;

namespace PG.StarWarsGame.Files.DAT.Services.Builder;

/// <summary>
/// Base class for a <see cref="IDatBuilder"/> service providing the fundamental implementations.
/// </summary>
public abstract class DatBuilderBase : FileBuilderBase<IReadOnlyList<DatStringEntry>, DatFileInformation>, IDatBuilder
{
    private readonly KeyValuePairList<string, DatStringEntry> _entries = new();

    private readonly ICrc32HashingService _hashingService;

    /// <inheritdoc />
    public sealed override IReadOnlyList<DatStringEntry> BuilderData =>
        TargetKeySortOrder == DatFileType.OrderedByCrc32 ? SortedEntries : Entries;

    /// <inheritdoc />
    public abstract DatFileType TargetKeySortOrder { get; }

    /// <inheritdoc />
    public abstract BuilderOverrideKind KeyOverwriteBehavior { get; }

    /// <inheritdoc />
    public IReadOnlyList<DatStringEntry> Entries => _entries.GetValueList();

    /// <inheritdoc />
    public IReadOnlyList<DatStringEntry> SortedEntries => Crc32Utilities.SortByCrc32(_entries.GetValueList()).ToList();

    /// <inheritdoc />
    public virtual IDatKeyValidator KeyValidator => NotNullKeyValidator.Instance;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatBuilderBase"/> class.
    /// </summary>
    /// <param name="services">The service provider.</param>
    protected DatBuilderBase(IServiceProvider services) : base(services)
    {
        _hashingService = Services.GetRequiredService<ICrc32HashingService>();
    }

    /// <inheritdoc />
    public AddEntryResult AddEntry(string key, string value)
    {
        ThrowIfDisposed();

        if (key == null) 
            throw new ArgumentNullException(nameof(key));
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        var encoding = DatFileConstants.TextKeyEncoding;
        var encodedKey = encoding.EncodeString(key, encoding.GetByteCountPG(key.Length));
        
        var keyValidation = KeyValidator.Validate(encodedKey);
        if (!keyValidation.IsValid)
            return AddEntryResult.EntryNotAdded(AddEntryState.InvalidKey, keyValidation.ToString());

        var crc = _hashingService.GetCrc32(encodedKey, encoding);

        var entry = new DatStringEntry(encodedKey, crc, value, key);

        var containsKey = _entries.ContainsKey(entry.Key, out var oldValue);

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
    public bool IsKeyValid(string key)
    {
        if (key == null) 
            throw new ArgumentNullException(nameof(key));
        return KeyValidator.Validate(key).IsValid;
    }

    /// <inheritdoc />
    protected sealed override void BuildFileCore(FileSystemStream fileStream, DatFileInformation fileInformation, IReadOnlyList<DatStringEntry> data)
    {
        var datService = Services.GetRequiredService<IDatFileService>();
        datService.CreateDatFile(fileStream, data, TargetKeySortOrder);
    }

    /// <inheritdoc />
    protected sealed override bool ValidateFileInformationCore(DatFileInformation fileInformation, IReadOnlyList<DatStringEntry> builderData,
        out string? failedReason)
    {
        failedReason = null;
        return true;
    }

    /// <inheritdoc />
    protected override void DisposeManagedResources()
    {
        base.DisposeManagedResources();
        _entries.Clear();
    }
}