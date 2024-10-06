// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace PG.StarWarsGame.Files.MTD.Data;

/// <summary>
/// The exception that is thrown when a Mega Texture entry is already present in the Mega Texture Directory collection.
/// </summary>
/// <param name="message">The error message that explains the reason for the exception.</param>
public sealed class DuplicateFileIndexException(string message) : Exception(message);