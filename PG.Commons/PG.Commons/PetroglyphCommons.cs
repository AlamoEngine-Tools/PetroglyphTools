// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Microsoft.Extensions.DependencyInjection;

namespace PG.Commons;

/// <summary>
///     Provides initialization routines for this library.
/// </summary>
public static class PetroglyphCommons
{
    /// <summary>
    ///     Adds all necessary services provided by this library to the specified <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
    public static void ContributeServices(IServiceCollection serviceCollection)
    {
    }
}
