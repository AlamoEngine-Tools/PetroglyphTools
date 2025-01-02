// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;

namespace PG.Commons.Binary;

// Note: The Alamo specifications usually allows tables with *unsigned* int32 possible entries, however in .NET we are
// limited to *signed* int32 for indexing native list-like structures.  

/// <summary>
/// A read-only list-like table used by Petroglyph binary models to hold index-based metadata information.
/// </summary>
/// <typeparam name="T">The type of the binary elements of the table.</typeparam>
public interface IBinaryTable<out T> : IBinary, IReadOnlyList<T> where T : IBinary;