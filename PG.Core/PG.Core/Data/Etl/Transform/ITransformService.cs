// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using PG.Core.Data.Etl.Stage.Bean;
using PG.Core.Services;

namespace PG.Core.Data.Etl.Transform
{
    public interface ITransformService<TStage1Bean, TStage2Bean> : IService
        where TStage1Bean : IStageBean
        where TStage2Bean : IStageBean
    {
        IReadOnlyList<TStage1Bean> Stage1Beans { get; }
        IReadOnlyList<TStage2Bean> Stage2Beans { get; }
        IReadOnlyList<TStage2Bean> BadBeans { get; }

        public void Transform();
        public bool TryTransformItem(TStage1Bean s1, out TStage2Bean s2);

        public string CreateTransformException(string message, string propertyStage1, string propertyStage2,
            Exception e = null);
    }
}
