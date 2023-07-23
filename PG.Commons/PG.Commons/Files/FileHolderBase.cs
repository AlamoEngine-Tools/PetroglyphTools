// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.IO.Abstractions;
using PG.Commons.Utilities;
using System.Runtime.CompilerServices;

namespace PG.Commons.Files;

/// <summary>
/// Wrapper around alamo file types that holds the file content in an accessible data structure.
/// </summary>
/// <typeparam name="TModel">The data model of the alamo file in a usable data format.</typeparam>
/// <typeparam name="TFileType">The alamo file type definition implementing <see cref="IAlamoFileType"/></typeparam>
public abstract class FileHolderBase<TModel, TFileType> : DisposableObject, IFileHolder<TModel, TFileType>
    where TFileType : IAlamoFileType, new()
{
    /// <inheritdoc/>
    public string Directory { get; }

    /// <inheritdoc/>
    public string FileName { get; }

    /// <inheritdoc/>
    public TFileType FileType { get; } = new();

    /// <inheritdoc/>
    public TModel Content { get; }

    /// <inheritdoc/>
    public string FilePath { get; }

    /// <summary>
    /// The logger of this service.
    /// </summary>
    protected internal ILogger? Logger { get; }

    /// <summary>
    /// The file system implementation to be used.
    /// </summary>
    protected internal IFileSystem FileSystem { get; }

    /// <summary>
    /// The service provider.
    /// </summary>
    protected internal IServiceProvider Services { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileHolderBase{TContent,TFileType}"/> class.
    /// </summary>
    /// <param name="model">The data model of this holder.</param>
    /// <param name="filePath">The path to the file on disc.</param>
    /// <param name="serviceProvider">The <see cref="IServiceProvider"/> for this instance.</param>
    /// <exception cref="ArgumentNullException">When any parameter is <see langword="null"/>.</exception>
    /// <exception cref="FileNotFoundException">When <paramref name="filePath"/> is not found.</exception>
    protected FileHolderBase(TModel model, string filePath, IServiceProvider serviceProvider)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentException("File path must not be null or empty.", nameof(filePath));
        Services = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        FileSystem = serviceProvider.GetRequiredService<IFileSystem>();
        Logger = serviceProvider.GetService<ILoggerFactory>()?.CreateLogger(GetType());
        Content = model ?? throw new ArgumentNullException(nameof(model));
        FilePath = filePath;


        // We do not check for the file extensions, since it's well possible users provide files with other extensions,
        // still representing the correct model. 
        // The game supports this as well, e.g. loading file "abc.def" as a meg archive, works just fine.

        // This call already throws an FileNotFoundException if the path does not exist at all.
        if (IsDirectorySeparator(filePath[filePath.Length - 1]) || FileSystem.File.GetAttributes(filePath).HasFlag(FileAttributes.Directory))
            throw new ArgumentException("Given file path is a directory.", nameof(filePath));

        // Empty file names such as "Data/.meg" are allowed in general. Thus we don't check for this constraint here.
        // The concrete holder can check for possible file name constraints.
        FileName = FileSystem.Path.GetFileNameWithoutExtension(filePath);

        Directory = FileSystem.Path.GetDirectoryName(filePath) ?? throw new InvalidOperationException("No directory found for file holder.");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsDirectorySeparator(char c)
    {
        return c == FileSystem.Path.DirectorySeparatorChar || c == FileSystem.Path.AltDirectorySeparatorChar;
    }
}