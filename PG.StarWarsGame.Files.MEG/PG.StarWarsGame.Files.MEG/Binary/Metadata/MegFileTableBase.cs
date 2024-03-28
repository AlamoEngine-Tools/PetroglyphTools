// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections;
using System.Collections.Generic;
using PG.Commons.Binary;

namespace PG.StarWarsGame.Files.MEG.Binary.Metadata;

internal abstract class MegFileTableBase<T>(IList<T> megFileContentTableRecords)
    : BinaryTable<T>(megFileContentTableRecords), IMegFileTable
    where T : IMegFileDescriptor
{
    IMegFileDescriptor IReadOnlyList<IMegFileDescriptor>.this[int i] => this[i];

    IEnumerator<IMegFileDescriptor> IEnumerable<IMegFileDescriptor>.GetEnumerator()
    {
        return new GenericMegFileDescriptorEnumerator<T>(Items);
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