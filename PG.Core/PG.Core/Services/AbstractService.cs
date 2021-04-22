// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Reflection;
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

        [JetBrains.Annotations.NotNullAttribute] protected internal ILogger<T> Logger { get; }

        /// <summary>
        /// The file system implementation to be used.
        /// Instead of relying on <see cref="System.IO"/> functionality this field should be used,
        /// as it can replaced by a mock implementation in tests quite easily.
        /// </summary>
        /// <example>
        /// FileSystem.Path.Combine("c:/my", "path");
        /// </example>
        protected internal IFileSystem FileSystem => m_fileSystem;

        /// <summary>
        /// Base .ctor
        /// </summary>
        /// <param name="fileSystem">
        /// The file system to be used. Initialised with the base file system <see cref="System.IO"/> if null is passed.
        /// </param>
        /// <param name="loggerFactory">
        /// The logger factory to be used to create the service <see cref="Logger"/>.
        /// Initialised via the <see cref="NullLogger"/> if not provided. 
        /// </param>
        protected AbstractService([JetBrains.Annotations.CanBeNullAttribute] IFileSystem fileSystem = null, [JetBrains.Annotations.CanBeNullAttribute] ILoggerFactory loggerFactory = null)
        {
            m_fileSystem = fileSystem ?? new FileSystem();
            loggerFactory ??= new NullLoggerFactory();
            Logger = loggerFactory.CreateLogger<T>();
        }

        #region IDisposable Pattern

        /// <summary>
        /// Internal <see cref="Dispose"/> function. 
        /// </summary>
        /// <param name="disposing">True when called by <see cref="Dispose"/></param>
        protected internal void DisposeInternal(bool disposing)
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

        
        /// <summary>
        /// Disposes of private object references as part of the internal <see cref="IDisposable"/> implementation.
        /// Currently only disposes of properties accessible via reflection.
        /// Collects all properties with <see cref="BindingFlags.NonPublic"/> and <see cref="BindingFlags.Instance"/> 
        /// </summary>
        protected internal void DisposePrivateObjectReferencesInternal()
        {
            foreach (PropertyInfo property in GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                object value0 = property.GetValue(this);
                if (value0 == null)
                {
                    continue;
                }
                if (!property.GetType().IsInstanceOfType(typeof(IDisposable)))
                {
                    continue;
                }
                IDisposable value = value0 as IDisposable;
                value?.Dispose();
            }
        }

        /// <summary>
        /// Releases unmanaged resources as part of the internal <see cref="IDisposable"/> implementation.
        /// </summary>
        protected internal void ReleaseUnmanagedResourcesInternal()
        {
            // NOP
        }

        /// <summary>
        /// Nulls large fields as part of the internal <see cref="IDisposable"/> implementation.
        /// </summary>
        protected internal void NullLargeFieldsInternal()
        {
            m_fileSystem = null;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            DisposeInternal(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
