// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace PG.Core.Services.Attributes
{
    /// <summary>
    /// An attribute that can be used on an interface to mark a given <see cref="Type"/> as the default (or fallback) implementation for a service.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    [ExcludeFromCodeCoverage]
    public sealed class DefaultServiceImplementationAttribute : Attribute
    {
        public Type DefaultServiceImplementation { get; }

        public DefaultServiceImplementationAttribute([JetBrains.Annotations.NotNull] Type defaultServiceImplementation)
        {
            Debug.Assert(typeof(IService).IsAssignableFrom(defaultServiceImplementation), $"The given type {defaultServiceImplementation} is not marked as {nameof(IService)}.");
            DefaultServiceImplementation = defaultServiceImplementation;
        }
    }
}
