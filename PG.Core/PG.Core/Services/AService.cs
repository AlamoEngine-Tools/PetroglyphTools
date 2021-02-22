// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.IO.Abstractions;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace PG.Core.Services
{
    /// <summary>
    /// A basic service implementation that requires a <see cref="IFileSystem"/> does expose a nullable <see cref="ILogger"/>.
    /// </summary>
    /// <typeparam name="T">The type of the service extending the base class.</typeparam>
    public abstract class AService<T> : IService
    {
        /// <summary>
        /// The file system implementation used for the service. Can be mocked or replaced via DI.
        /// </summary>
        [NotNull] protected readonly IFileSystem FileSystem;

        /// <summary>
        /// The service's logger implementation of type ILogger&lt;T&gt;
        /// </summary>
        [CanBeNull] protected readonly ILogger<T> Logger;

        protected AService([CanBeNull] IFileSystem fileSystem, [CanBeNull] ILoggerFactory loggerFactory)
        {
            FileSystem = fileSystem ?? new FileSystem();
            Logger = loggerFactory?.CreateLogger<T>();
        }
    }
}
