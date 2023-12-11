// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace PG.Commons.Utilities;

/// <summary>
/// Contains helpers for throwing common exceptions.
/// </summary>
public static class ThrowHelper
{
    /// <summary>
    /// Throws an exception if <paramref name="argument"/> is null, empty, or consists only of white-space characters.
    /// </summary>
    /// <param name="argument">The string argument to validate.</param>
    /// <param name="paramName">The name of the parameter with which <paramref name="argument"/> corresponds.</param>
    /// <exception cref="ArgumentNullException"><paramref name="argument"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="argument"/> is empty or consists only of white-space characters.</exception>
#pragma warning disable CS8777 // Parameter must have a non-null value when exiting.
    public static void ThrowIfNullOrWhiteSpace([NotNull] string? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (string.IsNullOrWhiteSpace(argument))
            ThrowNullOrWhiteSpaceException(argument, paramName);
    }
#pragma warning restore CS8777 // Parameter must have a non-null value when exiting.

    [DoesNotReturn]
    private static void ThrowNullOrWhiteSpaceException(string? argument, string? paramName = null)
    {
        if (argument is null)
            throw new ArgumentNullException(paramName);
        throw new ArgumentException("The value cannot be an empty string or composed entirely of whitespace.", paramName);
    }


    /// <summary>
    /// Throws an exception <see cref="ArgumentException"/> that <paramref name="argument"/> is not sorted in the expected way.
    /// </summary>
    /// <typeparam name="T">The type of the elements of the collection.</typeparam>
    /// <param name="argument">The collection which is not sorted.</param>
    /// <param name="paramName">The name of the parameter with which <paramref name="argument"/> corresponds.</param>
    /// <exception cref="ArgumentException"><paramref name="argument"/> is not sorted.</exception>
    [DoesNotReturn]
    public static void ThrowArgumentNotSortedException<T>(IEnumerable<T> argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        throw new ArgumentException("Collection is not correctly sorted.", paramName);
    }
}