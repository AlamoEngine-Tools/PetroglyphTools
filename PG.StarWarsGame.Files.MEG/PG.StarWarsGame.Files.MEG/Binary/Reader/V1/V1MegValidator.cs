// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.StarWarsGame.Files.MEG.Binary.Metadata.V1;
using PG.StarWarsGame.Files.MEG.Binary.Validation;

namespace PG.StarWarsGame.Files.MEG.Binary.V1;

// NB: Since this validator is used for READING MEG files only, we do not validate if the MEG file is larger than 2GB here.
// This allows this library to read larger MEG files and possible re-create them into smaller MEG files.
internal sealed class V1MegValidator(IServiceProvider serviceProvider) : MegBinaryValidator<MegMetadata>(serviceProvider);