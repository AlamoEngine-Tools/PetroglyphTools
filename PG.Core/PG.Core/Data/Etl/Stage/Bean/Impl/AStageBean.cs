// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace PG.Core.Data.Etl.Stage.Bean.Impl
{
    public abstract class AStageBean : IStageBean
    {
        public virtual string Id => Guid.NewGuid().ToString();
    }
}
