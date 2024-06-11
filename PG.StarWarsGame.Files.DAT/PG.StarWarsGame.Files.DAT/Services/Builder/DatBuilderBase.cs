// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
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
    private readonly Crc32KeyedList<DatStringEntry> _entries;

    private readonly ICrc32HashingService _hashingService;

    /// <inheritdoc />
    public sealed override IReadOnlyList<DatStringEntry> BuilderData =>
        TargetKeySortOrder == DatFileType.OrderedByCrc32 ? SortedEntries : Entries;

    /// <inheritdoc />
    public abstract DatFileType TargetKeySortOrder { get; }

    /// <inheritdoc />
    public BuilderOverrideKind KeyOverwriteBehavior { get; }

    /// <inheritdoc />
    public IReadOnlyList<DatStringEntry> Entries => _entries.GetValueList();

    /// <inheritdoc />
    public IReadOnlyList<DatStringEntry> SortedEntries => Crc32Utilities.SortByCrc32(_entries.GetValueList()).ToList();

    /// <inheritdoc />
    public virtual IDatKeyValidator KeyValidator => NotNullKeyValidator.Instance;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatBuilderBase"/> class.
    /// </summary>
    /// <param name="overrideKind"></param>
    /// <param name="services">The service provider.</param>
    protected DatBuilderBase(BuilderOverrideKind overrideKind, IServiceProvider services) : base(services)
    {
        KeyOverwriteBehavior = overrideKind;
        _hashingService = Services.GetRequiredService<ICrc32HashingService>();
        _entries = new Crc32KeyedList<DatStringEntry>(KeyOverwriteBehavior);
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


        scoped var encodedKey = key.AsSpan();

        var requiredLength = encoding.GetByteCountPG(key.Length);
        var keyBuffer = requiredLength > 260
            ? new char[requiredLength]
            : stackalloc char[requiredLength];

        encoding.Encode(encodedKey, keyBuffer, out var actualLength);
        if (actualLength != requiredLength)
            throw new InvalidOperationException("Encoding produces invalid string.");

        encodedKey = keyBuffer;

        var keyValidation = KeyValidator.Validate(encodedKey);
        if (!keyValidation)
            return AddEntryResult.NotAdded(AddEntryState.InvalidKey, "The key is not valid.");

        var crc = _hashingService.GetCrc32(encodedKey, encoding);

        var containsKey = _entries.ContainsKey(crc, out var existingValue);

        if (containsKey && KeyOverwriteBehavior == BuilderOverrideKind.NoOverwrite)
            return AddEntryResult.NotAddedDuplicate(existingValue);


        var entry = new DatStringEntry(encodedKey.ToString(), crc, value, key);

        if (KeyOverwriteBehavior == BuilderOverrideKind.AllowDuplicate)
        {
            _entries.AddOrReplace(crc, entry);
            return AddEntryResult.EntryAdded(entry, containsKey);
        }

        _entries.AddOrReplace(crc, entry);
        return AddEntryResult.EntryAdded(entry, existingValue);
    }

    /// <inheritdoc />
    public bool Remove(DatStringEntry entry)
    {
        return _entries.Remove(entry.Crc32, entry);
    }

    /// <inheritdoc />
    public bool RemoveAllKeys(string key)
    {
        var crc = _hashingService.GetCrc32(key.AsSpan(), DatFileConstants.TextKeyEncoding);
        return _entries.RemoveAll(crc);
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
        return KeyValidator.Validate(key);
    }

    /// <inheritdoc />
    public IDatModel BuildModel()
    {
        if (TargetKeySortOrder == DatFileType.OrderedByCrc32)
            return new SortedDatModel(BuilderData);
        return new UnsortedDatModel(BuilderData);
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