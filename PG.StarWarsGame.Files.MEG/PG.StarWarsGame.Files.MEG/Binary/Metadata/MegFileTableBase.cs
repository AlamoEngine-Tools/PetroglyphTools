// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PG.Commons.Binary;

namespace PG.StarWarsGame.Files.MEG.Binary.Metadata;

internal abstract class MegFileTableBase<T> : BinaryBase, IMegFileTable where T : IMegFileDescriptor
{
    protected readonly IReadOnlyList<T> MegFileContentTableRecords;

    public T this[int i] => MegFileContentTableRecords[i];

    IMegFileDescriptor IReadOnlyList<IMegFileDescriptor>.this[int i] => this[i];

    public int Count => MegFileContentTableRecords.Count;

    protected MegFileTableBase(IList<T> megFileContentTableRecords)
    {
        if (megFileContentTableRecords is null)
            throw new ArgumentNullException(nameof(megFileContentTableRecords));
        MegFileContentTableRecords = megFileContentTableRecords.ToList();
    }

    protected override int GetSizeCore()
    {
        if (MegFileContentTableRecords.Count == 0)
            return 0;
        if (MegFileContentTableRecords.Count == 1)
            return MegFileContentTableRecords[0].Size;
        return MegFileContentTableRecords.Sum(megFileNameTableRecord => megFileNameTableRecord.Size);
    }

    protected override byte[] ToBytesCore()
    {
        if (Size == 0)
            return Array.Empty<byte>();
        var bytes = new List<byte>(Size);
        foreach (var megFileContentTableRecord in MegFileContentTableRecords)
            bytes.AddRange(megFileContentTableRecord.Bytes);
        return bytes.ToArray();
    }

    public IEnumerator<IMegFileDescriptor> GetEnumerator()
    {
        return new GenericMegFileDescriptorEnumerator<T>(MegFileContentTableRecords);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private struct GenericMegFileDescriptorEnumerator<TInput> : IEnumerator<IMegFileDescriptor> where TInput : IMegFileDescriptor
    {
        private readonly IReadOnlyList<TInput> _list;

        private int _position;
        private TInput _currentRecord;

        readonly object IEnumerator.Current => Current;

        public readonly IMegFileDescriptor Current => _currentRecord;

        internal GenericMegFileDescriptorEnumerator(IReadOnlyList<TInput> list)
        {
            _list = list;
            _position = 0;
            _currentRecord = default!;
        }

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

        public void Reset()
        {
            _currentRecord = default!;
            _position = 0;
        }

        public readonly void Dispose()
        {
        }
    }
}