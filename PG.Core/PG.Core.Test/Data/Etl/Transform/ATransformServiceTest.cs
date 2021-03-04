// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Core.Data.Etl.Stage.Bean;
using PG.Core.Data.Etl.Transform;

namespace PG.Core.Test.Data.Etl.Transform
{
    [TestClass]
    public abstract class ATransformServiceTest<TStage1Bean, TStage2Bean>
        where TStage1Bean : IStageBean
        where TStage2Bean : IStageBean
    {
        protected const long LOAD_ID = 1;
        private long m_rowId;
        protected long RowIdNext => m_rowId++;

        [TestInitialize]
        protected void TestInitialize()
        {
            m_rowId = 0;
        }
        
        protected List<TStage1Bean> GetPositiveStage1Beans()
        {
            List<TStage1Bean> stage1Beans = new List<TStage1Bean> {GetMinimalPositiveStage1Bean()};
            return stage1Beans;
        }

        protected List<TStage1Bean> GetStage1Beans()
        {
            List<TStage1Bean> beans = GetPositiveStage1Beans();
            beans.AddRange(GetBadBeans());
            return beans;
        }

        protected abstract TStage1Bean GetMinimalPositiveStage1Bean();
        protected abstract List<TStage1Bean> GetBadBeans();

        protected abstract ITransformService<TStage1Bean, TStage2Bean> GetService(List<TStage1Bean> beans);

        [TestMethod]
        public void Transform_Test__PositiveBeansTransformSuccessfully()
        {
            List<TStage1Bean> beans = GetPositiveStage1Beans();
            ITransformService<TStage1Bean, TStage2Bean> svc = GetService(beans);
            svc.Transform();
            Assert.AreEqual(0, svc.BadBeans.Count);
            Assert.AreEqual(GetPositiveStage1Beans().Count, svc.Stage2Beans.Count);
        }

        [TestMethod]
        public void Transform_Test__BadBeansTransformWithErrorState()
        {
            ITransformService<TStage1Bean, TStage2Bean> svc = GetService(GetStage1Beans());
            svc.Transform();
            Assert.AreEqual(GetBadBeans().Count, svc.BadBeans.Count);
            Assert.AreEqual(GetPositiveStage1Beans().Count, svc.Stage2Beans.Count);
        }
    }
}
