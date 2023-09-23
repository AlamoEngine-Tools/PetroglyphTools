// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using PG.Commons.Utilities;

namespace PG.Commons.Files;

/// <summary>
///     Wrapper around alamo file types that holds the file content in an accessible data structure.
/// </summary>
/// <typeparam name="TModel">The data model of the alamo file in a usable data format.</typeparam>
/// <typeparam name="TFileType">The alamo file type definition implementing <see cref="IAlamoFileType" /></typeparam>
/// <typeparam name="TParam">The <see cref="IFileHolderParam" /> used during creation.</typeparam>
public abstract class FileHolderBase<TParam, TModel, TFileType> : DisposableObject, IFileHolder<TModel, TFileType>
    where TParam : IFileHolderParam
    where TFileType : IAlamoFileType, new()
{
    /// <inheritdoc />
    public string Directory { get; }

    /// <inheritdoc />
    public string FileName { get; }

    /// <inheritdoc />
    public TFileType FileType { get; } = new();

    /// <inheritdoc />
    public TModel Content { get; }

    /// <inheritdoc />
    public string FilePath { get; }

    /// <summary>
    ///     The logger of this service.
    /// </summary>
    protected internal ILogger Logger { get; }

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
        if (model == null) 
            throw new ArgumentNullException(nameof(model));
        if (param == null) 
            throw new ArgumentNullException(nameof(param));

        Content = model;
        Services = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        FileSystem = serviceProvider.GetRequiredService<IFileSystem>();
        Logger = serviceProvider.GetService<ILoggerFactory>()?.CreateLogger(GetType()) ?? NullLogger.Instance;
        

        var filePath = param.FilePath;

        // We do not use FileNameUtilities.IsValidFileName() cause don't want a custom PG as a dependency
        // We let concrete holders or other services the opportunity to do validation.
        // This of course allows invalid file names (such as ".", "..", "\\", "/") to be used here, but the assumption is,
        // that reading or writing such a file will cause an exception anyway. 
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path of param is empty.", nameof(param));


        FilePath = filePath;

        // Empty file names such as "Data/.meg" are allowed in general. Thus we don't check for this constraint here.
        // The concrete holder can check for possible file name constraints.
        FileName = FileSystem.Path.GetFileNameWithoutExtension(filePath);

        // Remember: For a file path "myfile.txt" the path is empty but not null.
        Directory = FileSystem.Path.GetDirectoryName(filePath) ??
                    throw new InvalidOperationException($"No directory found for file '{filePath}'");
    }
}