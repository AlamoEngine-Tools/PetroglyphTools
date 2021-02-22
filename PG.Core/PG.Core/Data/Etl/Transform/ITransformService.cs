// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using PG.Core.Services;

namespace PG.Core.Data.Etl.Transform
{
    public interface ITransformService<TStage1, TStage2> : IService
    {
        IReadOnlyList<TStage1> Stage1Beans { get; }
        IReadOnlyList<TStage2> Stage2Beans { get; }
        IReadOnlyList<TStage2> BadBeans { get; }

        public void Transform();
        public bool TryTransformItem(TStage1 s1, out TStage2 s2);
        public string CreateTransformException(string message, string propertyStage1, string propertyStage2, Exception e = null);

    }
}
