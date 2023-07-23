// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using PG.Commons.Utilities;

namespace PG.Commons.Services;

/// <summary>
/// Base class for services.
/// </summary>
public abstract class AbstractService : DisposableObject
{
    /// <summary>
    /// The logger of this service.<br/>
    /// May be of type <see cref="NullLogger"/> if no <see cref="ILoggerFactory"/> has been provided.
    /// </summary>
    protected internal ILogger Logger { get; }

    /// <summary>
    /// The file system implementation to be used.
    /// </summary>
    protected internal IFileSystem FileSystem { get; }

    /// <summary>
    /// The service provider.
    /// </summary>
    protected internal IServiceProvider Services { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractService"/> class.
    /// </summary>
    /// <param name="services">The service provider for this instance.</param>
    protected AbstractService(IServiceProvider services)
    {
        FileSystem = services.GetRequiredService<IFileSystem>();
        Logger = services.GetService<ILoggerFactory>()?.CreateLogger(GetType())?? NullLogger.Instance;
        Services = services;
    }
}
