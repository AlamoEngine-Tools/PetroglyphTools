// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using FluentValidation.Results;
using PG.Core.Data.Bean;
using PG.Core.Data.Validation;

namespace PG.StarWarsGame.Files.AnimationSfxMap.Data.Trigger
{
    public class SurfaceSfxTriggerBean : AbstractBean<SurfaceSfxTriggerKey>
    {
        public string Name { get; }

        public SurfaceSfxTriggerBean(SurfaceSfxTriggerKey key, string name) : base(key)
        {
            Name = name;
        }
    }
}
