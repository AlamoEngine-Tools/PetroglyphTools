// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.Commons.Files;
using PG.StarWarsGame.Files.MEG.Data.Archives;

namespace PG.StarWarsGame.Files.MEG.Files;

/// <inheritdoc cref="IMegFile" />
/// <remarks>
///     This class does not hold the actual data of the files packaged in a *.MEG file,
///     but all necessary meta-information to extract a requested file on-demand.
/// </remarks>
internal sealed class MegFile : PetroglyphFileHolder<IMegArchive, MegFileInformation>, IMegFile
{
    /// <inheritdoc/>
    public IMegArchive Archive => Content;

    /// <summary>
    /// Initializes a new instance of the <see cref="MegFile"/> class. 
    /// </summary>
    /// <remarks>
    /// It is safe to dispose the <paramref name="fileInformation"/> after an instance of this class has been created.
    /// </remarks>
    /// <param name="model">The meg archive model.</param>
    /// <param name="fileInformation">The initialization parameters.</param>
    /// <param name="serviceProvider">The service provider for this instance.</param>
    public MegFile(IMegArchive model, MegFileInformation fileInformation, IServiceProvider serviceProvider) :
        base(model, fileInformation, serviceProvider)
    {
    }
}