// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace PG.Commons.DataTypes;

/// <summary>
/// Base implementation for classes which shall implement the <see cref="IDisposable"/> interface.
/// This class provides convenience like a status flag and validation method.
/// <br/>
/// Disposable resources are divided in managed resources and unmanaged resources.
/// Managed resources get disposed when explicitly calling <see cref="Dispose()"/> on the instance.
/// Unmanaged resources additionally get disposed when the instance is finalized by the GC.
/// </summary>
public abstract class DisposableObject : IDisposable
{
    /// <summary>
    /// Indicates whether this instance is disposed.
    /// </summary>
    public bool IsDisposed { get; private set; }

    /// <inheritdoc cref="Finalize"/>
    ~DisposableObject()
    {
        Dispose(false);
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Throws an <see cref="ObjectDisposedException"/> if this instance already is disposed.
    /// </summary>
    protected void ThrowIfDisposed()
    {
        if (IsDisposed)
            throw new ObjectDisposedException(GetType().Name);
    }

    /// <summary>
    /// Disposes this instance and frees managed and unmanaged resources.
    /// Once this method is called <see cref="IsDisposed"/> is set to <see langword="true"/>
    /// </summary>
    /// <param name="disposing">When set to <see langword="true"/> managed resources get disposed.</param>
    protected void Dispose(bool disposing)
    {
        if (IsDisposed)
            return;
        try
        {
            if (disposing)
                DisposeManagedResources();
            DisposeNativeResources();
        }
        finally
        {
            IsDisposed = true;
        }
    }

    /// <summary>
    /// Disposes all managed resources.
    /// This method gets invoked if and only if the object's <see cref="Dispose()"/> method was called.
    /// </summary>
    protected virtual void DisposeManagedResources()
    {
    }

    /// <summary>
    /// Disposes all managed resources.
    /// This method gets invoked if the object gets disposed or finalized.
    /// </summary>
    protected virtual void DisposeNativeResources()
    {
    }
}