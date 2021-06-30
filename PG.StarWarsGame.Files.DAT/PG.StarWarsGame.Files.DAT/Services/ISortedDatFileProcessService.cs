// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Core.Attributes;
using PG.Core.Services.Attributes;
using PG.StarWarsGame.Files.DAT.Holder;

namespace PG.StarWarsGame.Files.DAT.Services
{
    /// <summary>
    /// MEF service interface definition for handling sorted *.DAT files.
    /// A default implementation is provided in <see cref="SortedDatFileProcessService"/>.
    /// When requesting the default implementation via an IoC Container or registering via injection, you may pass
    /// a file system as argument implementing <see cref="System.IO.Abstractions.IFileSystem"/> and a logger
    /// implementing <see cref="Microsoft.Extensions.Logging.ILogger"/>
    /// </summary>
    [Order(OrderAttribute.DEFAULT_ORDER)]
    [DefaultServiceImplementation(typeof(SortedDatFileProcessService))]
    public interface ISortedDatFileProcessService
    {
        /// <summary>
        /// Loads a *.DAT file from a provided and existing file on the file system
        /// and converts it into a <see cref="SortedDatFileHolder"/> instance.
        /// </summary>
        /// <param name="filePath">Path to an existing file on disc.
        /// If not validated properly any IO Exceptions may occur.</param>
        /// <returns>An instance of a <see cref="SortedDatFileHolder"/>.</returns>
        SortedDatFileHolder LoadFromFile(string filePath);

        /// <summary>
        /// Saves a given <see cref="SortedDatFileHolder"/> to disc.
        /// The save path provided with the holder must be valid, otherwise IO Exceptions may occur.
        /// </summary>
        /// <param name="sortedDatFileHolder">A valid <see cref="SortedDatFileHolder"/> instance.</param>
        void SaveToFile(SortedDatFileHolder sortedDatFileHolder);
    }
}
