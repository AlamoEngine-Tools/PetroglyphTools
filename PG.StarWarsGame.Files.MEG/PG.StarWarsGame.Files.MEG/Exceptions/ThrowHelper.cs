// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Diagnostics.CodeAnalysis;

namespace PG.StarWarsGame.Files.MEG;

internal static class ThrowHelper
{
    [DoesNotReturn]
    public static void ThrowDataEntryExceeds4GigabyteException(string? filePath)
    {
        var message = "Files larger than 4GB are not supported in MEG archives.";
        if (filePath is not null)
            message += $" File: '{filePath}'";

        throw new NotSupportedException(message);
    }

    [DoesNotReturn]
    public static void ThrowMegExceeds4GigabyteException(string? filePath)
    {
        var message = "MEG files larger than 4GB are not supported.";
        if (filePath is not null)
            message += $" File: '{filePath}'";
        throw new NotSupportedException(message);
    }

    [DoesNotReturn]
    public static void ThrowArgumentNotSortedException(string paramName)
    {
        throw new ArgumentException("Data entries are not sorted.", paramName);
    }
}

//internal static class ExceptionExtensions
//{
//    public static void ThrowIfNullOrWhiteSpace([NotNull] string? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
//    {
//        if (string.IsNullOrWhiteSpace(argument))
//            ThrowNullOrWhiteSpaceException(argument, paramName);
//    }

//    [DoesNotReturn]
//    private static void ThrowNullOrWhiteSpaceException(string? argument, string? paramName)
//    {
//        if (argument is null)
//            throw new ArgumentNullException(paramName);
//        throw new ArgumentException("The value cannot be an empty string or composed entirely of whitespace.", paramName);
//    }
//}

//namespace System.Runtime.CompilerServices
//{
//    [AttributeUsage(AttributeTargets.Parameter)]
//    internal sealed class CallerArgumentExpressionAttribute(string parameterName) : Attribute
//    {
//        public string ParameterName { get; } = parameterName;
//    }
//}