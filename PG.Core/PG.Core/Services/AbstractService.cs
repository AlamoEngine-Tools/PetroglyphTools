// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using PG.Core.Attributes;

namespace PG.Core.Services
{
    /// <summary>
    /// Base class for services.
    /// </summary>
    /// <typeparam name="T">The service extending the base.</typeparam>
    [Order(OrderAttribute.DEFAULT_ORDER)]
    [ExcludeFromCodeCoverage]
    public abstract class AbstractService<T> : IService, IDisposable
    {
        private bool m_isDisposed;
        private IFileSystem m_fileSystem;

        protected internal ILogger<T> Logger { get; }

        protected internal IFileSystem FileSystem => m_fileSystem;

        public AbstractService(IFileSystem fileSystem, ILoggerFactory loggerFactory)
        {
            m_fileSystem = fileSystem ?? new FileSystem();
            loggerFactory ??= new NullLoggerFactory();
            Logger = loggerFactory.CreateLogger<T>();
        }

        #region IDisposable Pattern

        protected internal virtual void DisposeInternal(bool disposing)
        {
            if (!m_isDisposed)
            {
                if (disposing)
                {
                    DisposePrivateObjectReferencesInternal();
                }
            }

            ReleaseUnmanagedResourcesInternal();
            NullLargeFieldsInternal();
            m_isDisposed = true;
        }

        protected internal void DisposePrivateObjectReferencesInternal()
        {
            // NOP
        }

        protected internal void ReleaseUnmanagedResourcesInternal()
        {
            // NOP
        }

        protected internal void NullLargeFieldsInternal()
        {
            m_fileSystem = null;
        }

        public void Dispose()
        {
            DisposeInternal(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
