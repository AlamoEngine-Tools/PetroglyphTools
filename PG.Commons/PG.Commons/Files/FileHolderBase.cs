// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.IO;
using System.IO.Abstractions;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PG.Commons.Utilities;

namespace PG.Commons.Files;

/// <summary>
///     Wrapper around alamo file types that holds the file content in an accessible data structure.
/// </summary>
/// <typeparam name="TModel">The data model of the alamo file in a usable data format.</typeparam>
/// <typeparam name="TFileType">The alamo file type definition implementing <see cref="IAlamoFileType" /></typeparam>
/// <typeparam name="TParam">The <see cref="IFileHolderParam" /> used during creation.</typeparam>
public abstract class FileHolderBase<TParam, TModel, TFileType> : DisposableObject,
    IFileHolder<TModel, TFileType>
    where TFileType : IAlamoFileType, new()
    where TParam : FileHolderParamBase
{
    /// <inheritdoc />
    public string Directory { get; private set; } = null!;

    /// <inheritdoc />
    public string FileName { get; private set; } = null!;

    /// <inheritdoc />
    public TFileType FileType { get; } = new();

    /// <inheritdoc />
    public TModel Content { get; }

    /// <inheritdoc />
    public string FilePath { get; private set; } = null!;

    /// <summary>
    ///     The logger of this service.
    /// </summary>
    protected internal ILogger? Logger { get; }

    /// <summary>
    ///     The file system implementation to be used.
    /// </summary>
    protected internal IFileSystem FileSystem { get; }

    /// <summary>
    ///     The service provider.
    /// </summary>
    protected internal IServiceProvider Services { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="FileHolderBase{TParam,TModel,TFileType}" /> class.
    /// </summary>
    /// <param name="model">The data model of this holder.</param>
    /// <param name="param">The creation param.</param>
    /// <param name="serviceProvider">The <see cref="IServiceProvider" /> for this instance.</param>
    /// <exception cref="ArgumentNullException">When any parameter is <see langword="null" />.</exception>
    protected FileHolderBase(TModel model, TParam param, IServiceProvider serviceProvider)
    {
        Services = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        FileSystem = serviceProvider.GetRequiredService<IFileSystem>();
        Logger = serviceProvider.GetService<ILoggerFactory>()?.CreateLogger(GetType());
        Content = model ?? throw new ArgumentNullException(nameof(model));

        ConstructInternal(param);
    }

    private void ConstructInternal(TParam param)
    {
        if (string.IsNullOrWhiteSpace(param.FilePath))
        {
            throw new ArgumentException("File path must not be null or empty.", nameof(param.FilePath));
        }

        // We do not check for the file extensions, since it's well possible users provide files with other extensions,
        // still representing the correct model. 
        // The game supports this as well, e.g. loading file "abc.def" as a meg archive, works just fine.

        // This call already throws an FileNotFoundException if the path does not exist at all.
        if (IsDirectorySeparator(param.FilePath![param.FilePath.Length - 1]))
        {
            throw new ArgumentException("Given file path is a directory.", nameof(param.FilePath));
        }

        FilePath = param.FilePath;

        // Empty file names such as "Data/.meg" are allowed in general. Thus we don't check for this constraint here.
        // The concrete holder can check for possible file name constraints.
        FileName = FileSystem.Path.GetFileNameWithoutExtension(param.FilePath);

        Directory = FileSystem.Path.GetDirectoryName(param.FilePath) ??
                    throw new InvalidOperationException("No directory found for file holder.");
        ConstructHook(param);
    }

    /// <summary>
    ///     Method hook for customized parameter handling.
    /// </summary>
    /// <param name="param">The parameter extension.</param>
    protected virtual void ConstructHook(TParam param)
    {
        // method hook.
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsDirectorySeparator(char c)
    {
        return (c == FileSystem.Path.DirectorySeparatorChar) || (c == FileSystem.Path.AltDirectorySeparatorChar);
    }
}