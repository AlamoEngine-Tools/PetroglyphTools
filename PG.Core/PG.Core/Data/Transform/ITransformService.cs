// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;

namespace PG.Core.Data.Transform
{
    public interface ITransformService<TStage1,TStage2>
    {
        IReadOnlyList<TStage1> Stage1Beans { get; }
        IReadOnlyList<TStage2> Stage2Beans { get; } 
    }
}
