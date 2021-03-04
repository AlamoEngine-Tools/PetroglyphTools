// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using PG.Core.Data.Etl.Stage.Bean;
using PG.Core.Services;

namespace PG.Core.Data.Etl.Load
{
    public interface ILoadService<TStage2Bean, TTargetBean> : IService
        where TStage2Bean : IStageBean
        where TTargetBean : IPersistenceEntity
    {
        public IReadOnlyList<TStage2Bean> Stage2Beans { get; }
        public IReadOnlyList<TTargetBean> TargetBeans { get; }
        public void Load();
        public bool TryLoadItem(TStage2Bean stage2Bean, out TTargetBean targetBean);
    }
}
