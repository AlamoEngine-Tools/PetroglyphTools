// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Core.Attributes;
using PG.Core.Reporting;
using PG.StarWarsGame.Files.AnimationSfxMap.Data.Trigger;
using System.Collections.Generic;

namespace PG.StarWarsGame.Files.AnimationSfxMap.Services
{
    [Order(OrderAttribute.DEFAULT_ORDER)]
    public interface ISurfaceTriggerProcessService
    {
        public IReport LoadFromXmlFile(string filePath, out IEnumerable<SurfaceSfxTriggerBean> surfaceSfxTriggers);
        public IEnumerable<SurfaceSfxTriggerBean> LoadDefault();
    }
}
