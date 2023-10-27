// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;

namespace PG.StarWarsGame.Files.MEG.Data;

/// <summary>
/// A fully in-memory representation of a MEG archive whose files may be distributed across multiple physical .MEG files or bare local files.
/// </summary>
/// <remarks>
/// The underlying <see cref="IMegArchive"/> is compliant to the specification and is ready to get converted to a binary representation.
/// </remarks>
// Unfortunately this interface cannot directly inherit from IMegArchive as otherwise an IMegFile could have a virtual archive as a model.
// The alternative would be to have IMegArchive as a base for IVirtualMegArchive and something like IPhysicalMegArchive.
// This however causes problems:
//      a) the IMegBinaryConverter cannot use the IMegArchive base interface. Instead it needs the more specific types for return and parameter types.
//      b) there needs to be a conversion IMegArchive --> IPhysicalMegArchive. However, the conversion is not always legal
//         e.g., IVirtualMegArchive --> IPhysicalMegArchive should not be allowed.
//         Ultimately this conversion would make this API hard to understand and use.
public interface IVirtualMegArchive
{
    /// <summary>
    /// Gets the archive model which represents this virtual archive.
    /// </summary>
    IReadOnlyList<string> Files { get; }

    /// <summary>
    /// Get the origin info for a given <see cref="MegDataEntry"/> of this archive. <see langword="null"/> if <paramref name="entry"/> was not found.
    /// </summary>
    /// <param name="entry">The data entry to get its location for.</param>
    /// <returns>The origin info of the entry or <see langword="null"/>.</returns>
    MegDataEntryOriginInfo? GetOrigin(MegDataEntry entry);
}


internal interface IMegConstructionArchive : IVirtualMegArchive
{
    /// <summary>
    /// Gets the archive model which represents this virtual archive.
    /// </summary>
    IMegArchive Archive { get; }
}