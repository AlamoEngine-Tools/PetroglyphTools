// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Core.Data;
using PG.Core.Data.Etl.Load;
using PG.Core.Data.Etl.Stage.Bean;

namespace PG.Core.Test.Data.Etl.Load
{
    [TestClass]
    public abstract class ALoadServiceTest<TStage2Bean, TTargetBean>
        where TStage2Bean : IStageBean
        where TTargetBean : IPersistenceEntity
    {
        protected List<TStage2Bean> GetStage2Beans()
        {
            List<TStage2Bean> beans = new List<TStage2Bean> {GetMinimalPositiveStage2Bean()};
            return beans;
        }

        protected abstract TStage2Bean GetMinimalPositiveStage2Bean();
        protected abstract ILoadService<TStage2Bean, TTargetBean> GetService(List<TStage2Bean> beans);

        [TestMethod]
        public void Load_Test__BeansLoadSuccessfully()
        {
            ILoadService<TStage2Bean, TTargetBean> svc = GetService(GetStage2Beans());
            svc.Load();
            Assert.AreEqual(GetStage2Beans().Count, svc.Stage2Beans.Count);
            foreach (TStage2Bean stage2Bean in GetStage2Beans())
            {
                Assert_HasTargetBean(stage2Bean, svc.TargetBeans);
            }
        }

        protected void Assert_HasTargetBean(TStage2Bean bean, IEnumerable<TTargetBean> targetBeans)
        {
            bool contains = false;
            foreach (TTargetBean persistenceEntity in targetBeans)
            {
                if (bean.Id.Equals(persistenceEntity.Id, StringComparison.InvariantCulture))
                {
                    contains = true;
                }
            }
            Assert.IsTrue(contains, $"The {nameof(IStageBean)} {nameof(bean)} [{bean}] was not loaded into the target.");    
        }
    }
}
