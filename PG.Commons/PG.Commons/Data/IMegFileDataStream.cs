// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace PG.Commons.Data;

/// <summary>
/// Identifies a type to represent a MEG data entry stream.
/// </summary>
public interface IMegFileDataStream
{
    /// <summary>
    /// Gets the path of the entry used in the MEG archive.
    /// </summary>
    string EntryPath { get; }
}