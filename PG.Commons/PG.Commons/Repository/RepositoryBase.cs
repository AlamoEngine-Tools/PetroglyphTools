// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using PG.Commons.Validation;

namespace PG.Commons.Repository;

/// <inheritdoc />
public abstract class RepositoryBase<TKey, TValue> : IRepository<TKey, TValue>
    where TKey : notnull
    where TValue :
    IValidatable
{
    /// <summary>
    ///     .ctor
    /// </summary>
    /// <param name="services"></param>
    protected RepositoryBase(IServiceProvider services)
    {
        Logger = services.GetService<ILoggerFactory>()?.CreateLogger(GetType()) ?? NullLogger.Instance;
    }


    /// <inheritdoc />
    public IReadOnlyCollection<TKey> Keys => InternalRepository.Keys;


    /// <inheritdoc />
    public IReadOnlyCollection<TValue> Values => InternalRepository.Values;

    /// <summary>
    ///     The internal Dictionary.
    /// </summary>
    protected readonly Dictionary<TKey, TValue> InternalRepository = new();

    /// <inheritdoc cref="ILogger{TCategoryName}" />
    protected readonly ILogger Logger;

    /// <inheritdoc />
    public ValidationResult Validate()
    {
        var errors = new List<ValidationFailure>();
        foreach (var validationFailures in InternalRepository.Values.Select(v => v.Validate().Errors))
        {
            errors.AddRange(validationFailures);
        }

        return new ValidationResult(errors);
    }

    /// <inheritdoc />
    public void ValidateAndThrow()
    {
        foreach (var v in InternalRepository.Values)
        {
            v.ValidateAndThrow();
        }
    }

    /// <inheritdoc />
    public bool TryCreate(TKey key, TValue value)
    {
        if (InternalRepository.ContainsKey(key))
        {
            Logger.LogInformation(
                "The repository already has a value set for the key={key}. The value could not be inserted.", key);
            return false;
        }

        var validationResult = value.Validate();
        if (!validationResult.IsValid)
        {
            Logger.LogWarning("The provided value {value} is invalid. Validation result: {validationResult}", value,
                validationResult);
            return false;
        }

        InternalRepository.Add(key, value);
        return true;
    }

    /// <inheritdoc />
    public bool TryRead(TKey key, out TValue? value)
    {
        if (!InternalRepository.ContainsKey(key))
        {
            Logger.LogInformation("There is no object with key={key} in repository.", key);
            value = default;
            return false;
        }

        value = InternalRepository[key];
        return true;
    }

    /// <inheritdoc />
    public bool TryUpdate(TKey key, TValue value)
    {
        if (!InternalRepository.ContainsKey(key))
        {
            Logger.LogInformation("There is no object with key={key} in repository.", key);
            return false;
        }

        var validationResult = value.Validate();
        if (!validationResult.IsValid)
        {
            Logger.LogWarning("The provided value {value} is invalid. Validation result: {validationResult}", value,
                validationResult);
            return false;
        }

        InternalRepository[key] = value;
        return true;
    }

    /// <inheritdoc />
    public bool TryDelete(TKey key)
    {
        if (!InternalRepository.ContainsKey(key))
        {
            Logger.LogInformation("There is no object with key={key} in repository.", key);
            return false;
        }

        return InternalRepository.Remove(key);
    }
}