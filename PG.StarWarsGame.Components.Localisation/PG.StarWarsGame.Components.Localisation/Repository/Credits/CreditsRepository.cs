// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;

namespace PG.StarWarsGame.Components.Localisation.Repository.Credits;

/// <summary>
///     The credits repository is _not_ a normal repository, as the credit files are abusing
///     the keys as formatting information for the credits text crawl.
/// </summary>
internal sealed class CreditsRepository : ICreditsRepository, IList<ITranslationItem>
{
    private readonly IList<ITranslationItem> _internalRepository = new List<ITranslationItem>();

    /// <inheritdoc />
    public ValidationResult Validate()
    {
        var errors = new List<ValidationFailure>();
        foreach (var validationFailures in _internalRepository.Select(v => v.Validate().Errors))
        {
            errors.AddRange(validationFailures);
        }

        return new ValidationResult(errors);
    }

    /// <inheritdoc />
    public void ValidateAndThrow()
    {
        foreach (var v in _internalRepository)
        {
            v.ValidateAndThrow();
        }
    }

    /// <inheritdoc />
    public IEnumerator<ITranslationItem> GetEnumerator()
    {
        return _internalRepository.GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_internalRepository).GetEnumerator();
    }

    public void Add(ITranslationItem item)
    {
        _internalRepository.Add(item);
    }

    public void Clear()
    {
        _internalRepository.Clear();
    }

    public bool Contains(ITranslationItem item)
    {
        return _internalRepository.Contains(item);
    }

    /// <inheritdoc />
    public void CopyTo(ITranslationItem[] array, int arrayIndex)
    {
        _internalRepository.CopyTo(array, arrayIndex);
    }

    public bool Remove(ITranslationItem item)
    {
        return _internalRepository.Remove(item);
    }

    /// <inheritdoc />
    public int Count => _internalRepository.Count;

    /// <inheritdoc />
    public bool IsReadOnly => _internalRepository.IsReadOnly;

    /// <inheritdoc />
    public int IndexOf(ITranslationItem item)
    {
        return _internalRepository.IndexOf(item);
    }

    /// <inheritdoc />
    public void Insert(int index, ITranslationItem item)
    {
        _internalRepository.Insert(index, item);
    }

    /// <inheritdoc />
    public void RemoveAt(int index)
    {
        _internalRepository.RemoveAt(index);
    }

    /// <inheritdoc />
    public ITranslationItem this[int index]
    {
        get => _internalRepository[index];
        set => _internalRepository[index] = value;
    }
}