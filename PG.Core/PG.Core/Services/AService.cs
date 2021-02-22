// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.IO.Abstractions;
using Microsoft.Extensions.Logging;

namespace PG.Core.Services
{
    public abstract class AService<T> : IService
    {
        protected readonly IFileSystem m_fileSystem;
        protected readonly ILogger<T> m_logger;

        protected AService(IFileSystem fileSystem, ILoggerFactory loggerFactory)
        {
            m_fileSystem = fileSystem ?? new FileSystem();
            m_logger = loggerFactory?.CreateLogger<T>();
        }
    }
}
