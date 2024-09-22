// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace PG.Commons.Utilities;

/// <summary>
/// Helper interface to identify a MEG data entry stream without having the MEG package as dependency
/// </summary>
public interface IMegFileDataStream
{
    /// <summary>
    /// Gets the path of the entry used the MEG archive.
    /// </summary>
    string EntryPath { get; }
}