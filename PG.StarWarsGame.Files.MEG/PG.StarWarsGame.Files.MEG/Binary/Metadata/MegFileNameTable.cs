// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using PG.Commons.Binary;
using PG.Commons.Collections;

namespace PG.StarWarsGame.Files.MEG.Binary.Metadata;

internal class MegFileNameTable : BinaryBase, IMegFileNameTable, IEnumerable<MegFileNameTableRecord>
{
    private readonly IReadOnlyList<MegFileNameTableRecord> _megFileNameTableRecords;

    public MegFileNameTableRecord this[int i] => _megFileNameTableRecords[i];

    MegFileNameInformation IBinaryTable<MegFileNameInformation>.this[int i]
    {
        get
        {
            var entry = _megFileNameTableRecords[i];
            return new MegFileNameInformation(entry.FileName, entry.OriginalFilePath);
        }
    }

    public int Count => _megFileNameTableRecords.Count;

    public MegFileNameTable(IList<MegFileNameTableRecord> megFileNameTableRecords)
    {
        if (megFileNameTableRecords is null)
            throw new ArgumentNullException(nameof(megFileNameTableRecords));
        if (megFileNameTableRecords.Count == 0)
            _megFileNameTableRecords = Array.Empty<MegFileNameTableRecord>();
        else
            _megFileNameTableRecords = megFileNameTableRecords.ToList();
    }

    protected override int GetSizeCore()
    {
        if (_megFileNameTableRecords.Count == 0)
            return 0;
        if (_megFileNameTableRecords.Count == 1)
            return _megFileNameTableRecords[0].Size;
        return _megFileNameTableRecords.Sum(megFileNameTableRecord => megFileNameTableRecord.Size);
    }

    protected override byte[] ToBytesCore()
    {
        if (Size == 0)
            return Array.Empty<byte>();
        var bytes = new List<byte>(Size);
        foreach (var megFileNameTableRecord in _megFileNameTableRecords)
            bytes.AddRange(megFileNameTableRecord.Bytes);
        return bytes.ToArray();
    }

    IEnumerator<MegFileNameInformation> IEnumerable<MegFileNameInformation>.GetEnumerator()
    {
        return new MegFileNameInformationEnumerator(_megFileNameTableRecords);
    }

    public IEnumerator<MegFileNameTableRecord> GetEnumerator()
    {
        return _megFileNameTableRecords.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }


    /// <summary>
    /// Enumerates the elements of a <see cref="FrugalList{T}"/>.
    /// </summary>
    public struct MegFileNameInformationEnumerator : IEnumerator<MegFileNameInformation>
    {
        private readonly IReadOnlyList<MegFileNameTableRecord> _list;

        private int _position;
        private MegFileNameTableRecord _currentRecord;

        readonly object IEnumerator.Current => Current!;

        /// <inheritdoc />
        public readonly MegFileNameInformation Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                var currentRecord = _currentRecord;
                return new MegFileNameInformation(currentRecord.FileName, currentRecord.OriginalFilePath);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal MegFileNameInformationEnumerator(IReadOnlyList<MegFileNameTableRecord> list)
        {
            _list = list;
            _position = 0;
            _currentRecord = default!;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            if (_position < _list.Count)
            {
                _currentRecord = _list[_position];
                ++_position;
                return true;
            }
            _position = _list.Count + 1;
            _currentRecord = default!;
            return false;
        }

        /// <inheritdoc />
        public void Reset()
        {
            _currentRecord = default!;
            _position = 0;

        }

        /// <inheritdoc />
        public readonly void Dispose()
        {
        }
    }
}