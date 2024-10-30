// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using PG.StarWarsGame.Components.Localisation.Languages;
using PG.StarWarsGame.Components.Localisation.Repository.Content;

namespace PG.StarWarsGame.Components.Localisation.Repository.Builtin;

/// <summary>
///     A simple ordered in-memory translation repository
/// </summary>
public class InMemoryOrderedTranslationRepository : ITranslationRepository
{
    private IDictionary<IAlamoLanguageDefinition, IDictionary<OrderedTranslationItemId, ITranslationItem>>
        Repository { get; } =
        new Dictionary<IAlamoLanguageDefinition, IDictionary<OrderedTranslationItemId, ITranslationItem>>();

    /// <inheritdoc />
    public IReadOnlyDictionary<IAlamoLanguageDefinition, ICollection<ITranslationItem>> Content => ToContent();

    /// <inheritdoc />
    public bool AddLanguage(IAlamoLanguageDefinition language)
    {
        try
        {
            Repository.Add(language, new Dictionary<OrderedTranslationItemId, ITranslationItem>());
            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }

    /// <inheritdoc />
    public bool AddOrUpdateLanguage(IAlamoLanguageDefinition language,
        ICollection<ITranslationItem> translationItems,
        ITranslationRepository.MergeStrategy strategy = ITranslationRepository.MergeStrategy.MergeByKey)
    {
        return strategy switch
        {
            ITranslationRepository.MergeStrategy.Replace => AddOrUpdateLanguageReplace(language, translationItems),
            ITranslationRepository.MergeStrategy.MergeByKey => AddOrUpdateLanguageMergeByKey(language,
                translationItems),
            ITranslationRepository.MergeStrategy.Append => AddOrUpdateLanguageAppend(language, translationItems),
            _ => throw new ArgumentOutOfRangeException(nameof(strategy), strategy, null)
        };
    }

    /// <inheritdoc />
    public bool RemoveLanguage(IAlamoLanguageDefinition language)
    {
        return Repository.Remove(language);
    }

    /// <inheritdoc />
    public ITranslationItem GetTranslationItem(IAlamoLanguageDefinition language, ITranslationItemId id)
    {
        if (id is not OrderedTranslationItemId key)
            throw new ArgumentException(
                $"The passed id is of type {id.GetType()} but should be of type {nameof(OrderedTranslationItemId)}",
                nameof(id));
        if (!Repository.TryGetValue(language, out var translationItems))
            throw new KeyNotFoundException($"The language {language} was not found");
        if (!translationItems.TryGetValue(key, out var item))
            throw new KeyNotFoundException($"The item {id} was not found");
        return item;
    }

    /// <inheritdoc />
    public bool RemoveTranslationItem(ITranslationItemId id)
    {
        if (id is not OrderedTranslationItemId key)
            throw new ArgumentException(
                $"The passed id is of type {id.GetType()} but should be of type {nameof(OrderedTranslationItemId)}",
                nameof(id));
        return Repository.Values.Where(i => i.ContainsKey(key))
            .Aggregate(false, (current, i) => current | i.Remove(key));
    }

    /// <inheritdoc />
    public void AddOrUpdateTranslationItem(IAlamoLanguageDefinition language, ITranslationItem item)
    {
        if (item.ItemId is not OrderedTranslationItemId key)
            throw new ArgumentException(
                $"The passed id is of type {item.ItemId} but should be of type {nameof(OrderedTranslationItemId)}",
                nameof(item.ItemId));
        if (!Repository.TryGetValue(language, out var translationItems))
            throw new KeyNotFoundException($"The language {language} was not found");
        translationItems[key] = item;
    }

    /// <inheritdoc />
    public ITranslationDiff CreateTranslationDiff(IAlamoLanguageDefinition diffBase)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public bool ApplyTranslationDiff(ITranslationDiff diff)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public IReadOnlyCollection<ITranslationItem> GetTranslationItemsByLanguage(
        IAlamoLanguageDefinition language)
    {
        if (!Repository.TryGetValue(language, out var value))
            throw new KeyNotFoundException($"The provided language {language} could not be found.");

        return value.Values.ToImmutableList();
    }

    private IReadOnlyDictionary<IAlamoLanguageDefinition, ICollection<ITranslationItem>> ToContent()
    {
        return Repository
            .ToImmutableDictionary<
                KeyValuePair<IAlamoLanguageDefinition, IDictionary<OrderedTranslationItemId, ITranslationItem>>,
                IAlamoLanguageDefinition, ICollection<ITranslationItem>>(pair => pair.Key,
                pair => new List<ITranslationItem>(pair.Value.Values));
    }

    private bool AddOrUpdateLanguageAppend(IAlamoLanguageDefinition language,
        ICollection<ITranslationItem> translationItems)
    {
        if (!Repository.TryGetValue(language, out var repositoryItems))
        {
            Repository.Add(language, new Dictionary<OrderedTranslationItemId, ITranslationItem>());
            repositoryItems = Repository[language];
        }

        var itemsToAdd = new Dictionary<OrderedTranslationItemId, ITranslationItem>();
        foreach (var item in translationItems)
        {
            if (item.ItemId is not OrderedTranslationItemId key)
                throw new ArgumentException(
                    $"The passed id is of type {item.ItemId} but should be of type {nameof(OrderedTranslationItemId)}",
                    nameof(item.ItemId));
            if (!repositoryItems.ContainsKey(key))
                itemsToAdd.Add(key, item);
        }

        foreach (var i in itemsToAdd) repositoryItems.Add(i.Key, i.Value);

        return translationItems.Count == itemsToAdd.Count;
    }

    private bool AddOrUpdateLanguageMergeByKey(IAlamoLanguageDefinition language,
        ICollection<ITranslationItem> translationItems)
    {
        if (!Repository.TryGetValue(language, out var repositoryItems))
        {
            Repository.Add(language, new Dictionary<OrderedTranslationItemId, ITranslationItem>());
            repositoryItems = Repository[language];
        }

        var itemsToAdd = new Dictionary<OrderedTranslationItemId, ITranslationItem>();
        foreach (var item in translationItems)
        {
            if (item.ItemId is not OrderedTranslationItemId key)
                throw new ArgumentException(
                    $"The passed id is of type {item.ItemId} but should be of type {nameof(OrderedTranslationItemId)}",
                    nameof(item.ItemId));
            if (repositoryItems.ContainsKey(key))
                repositoryItems[key] = item;
            else
                itemsToAdd.Add(key, item);
        }

        foreach (var i in itemsToAdd) repositoryItems.Add(i.Key, i.Value);

        return true;
    }

    private bool AddOrUpdateLanguageReplace(IAlamoLanguageDefinition language,
        ICollection<ITranslationItem> translationItems)
    {
        if (!Repository.TryGetValue(language, out var repositoryItems))
        {
            Repository.Add(language, new Dictionary<OrderedTranslationItemId, ITranslationItem>());
            repositoryItems = Repository[language];
        }

        repositoryItems.Clear();
        foreach (var item in translationItems)
        {
            if (item.ItemId is not OrderedTranslationItemId key)
                throw new ArgumentException(
                    $"The passed id is of type {item.ItemId} but should be of type {nameof(OrderedTranslationItemId)}",
                    nameof(item.ItemId));
            repositoryItems.Add(key, item);
        }

        return true;
    }
}
