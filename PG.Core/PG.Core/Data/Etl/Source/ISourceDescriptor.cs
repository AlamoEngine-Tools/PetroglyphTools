// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace PG.Core.Data.Etl.Source
{
    public interface ISourceDescriptor
    {
        string SourcePath { get; }
        Type ConfiguredService { get; }
    }
}
