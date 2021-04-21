// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Core.Data;
using PG.Core.Data.Bean;
using PG.Core.Data.Key;
using PG.Core.Data.Repository;

namespace PG.Core.Test.Data.Repository
{
    [TestClass]
    public abstract class AbstractInMemoryKeyValuePairRepositoryTest<TRepository, TKey, TBean>
        where TRepository : AbstractInMemoryKeyValuePairRepository<TKey, TBean> where TKey : IKey where TBean : IBean<TKey>
    {
        protected internal virtual TRepository CreateRepository()
        {
            return Activator.CreateInstance<TRepository>();
        }

        protected internal abstract IEnumerable<TBean> GetConfiguredUniqueBeans();
        protected internal abstract TBean GetConfiguredSingleBean();

        protected internal virtual TBean GetConfiguredNullBean()
        {
            return default(TBean);
        }
        
        protected internal virtual TKey GetConfiguredNullKey()
        {
            return default(TKey);
        }

        [TestMethod]
        public void Test__TestSetup()
        {
            TestTestSetupInternal();
        }

        [TestMethod]
        public void Test_CRUD__OnNullBean()
        {
            TestCrudOnNullBeanInternal();
        }

        [TestMethod]
        public void Test_CRUD__OnSingleBean()
        {
            TestCrudOnSingleBeanInternal();
        }

        [TestMethod]
        public void Test_CRUD__OnUniqueBeans()
        {
            TestCrudOnUniqueBeansInternal();
        }

        private protected virtual void TestTestSetupInternal()
        {
            using TRepository repository = CreateRepository();
            Assert.IsNotNull(repository);
            Assert.IsTrue(repository is IRepository<TKey, TBean>);
            TBean bean = GetConfiguredSingleBean();
            Assert.IsNotNull(bean);
            Assert.IsTrue(bean is IBean<TKey>);
            IEnumerable<TBean> beans = GetConfiguredUniqueBeans();
            Assert.IsNotNull(beans);
            Assert.IsTrue(beans.Any());
            foreach (TBean b in beans)
            {
                Assert.IsNotNull(b);
                Assert.IsTrue(b is IBean<TKey>);
            }
        }

        protected internal virtual void TestCrudOnNullBeanInternal()
        {
            using TRepository repository = CreateRepository();
            TBean bean = GetConfiguredNullBean();
            Assert.IsNotNull(repository);
            Assert.IsNull(bean);
            Assert.IsFalse(repository.TryAdd(bean));
            Assert.IsFalse(repository.TryUpdate(bean));
            Assert.IsFalse(repository.TryAddOrUpdate(bean));
            Assert.IsFalse(repository.TryGet(GetConfiguredNullKey(), out TBean b));
            Assert.IsNull(b);
            Assert.IsFalse(repository.TryRemove(bean));
        }

        protected internal virtual void TestCrudOnSingleBeanInternal()
        {
            using TRepository repository = CreateRepository();
            TBean bean = GetConfiguredSingleBean();
            Assert.IsNotNull(repository);
            Assert.IsNotNull(bean);
            AssertCrudOnSingleBean(repository, bean);
        }

        protected internal virtual void TestCrudOnUniqueBeansInternal()
        {
            using TRepository repository = CreateRepository();
            IEnumerable<TBean> beans = GetConfiguredUniqueBeans().ToList();
            foreach (TBean bean in beans)
            {
                Assert.IsTrue(repository.TryAdd(bean));
            }

            foreach (TBean bean in beans)
            {
                Assert.IsTrue(repository.TryGet(bean.Key, out TBean bean0));
                Assert.Equals(bean, bean0);
            }

            foreach (TBean bean in beans)
            {
                Assert.IsTrue(repository.TryUpdate(bean));
            }

            foreach (TBean bean in beans)
            {
                Assert.IsTrue(repository.TryGet(bean.Key, out TBean bean0));
                Assert.Equals(bean, bean0);
            }

            foreach (TBean bean in beans)
            {
                Assert.IsTrue(repository.TryAddOrUpdate(bean));
            }

            foreach (TBean bean in beans)
            {
                Assert.IsTrue(repository.TryGet(bean.Key, out TBean bean0));
                Assert.Equals(bean, bean0);
            }

            foreach (TBean bean in beans)
            {
                Assert.IsFalse(repository.TryAdd(bean));
            }

            foreach (TBean bean in beans)
            {
                Assert.IsTrue(repository.TryRemove(bean));
            }

            foreach (TBean bean in beans)
            {
                Assert.IsFalse(repository.TryGet(bean.Key, out _));
            }
        }

        protected internal virtual void AssertCrudOnSingleBean(TRepository repository, TBean bean)
        {
            Assert.IsTrue(repository.TryAdd(bean));
            {
                Assert.IsTrue(repository.TryGet(bean.Key, out TBean bean0));
                Assert.Equals(bean, bean0);
            }
            Assert.IsTrue(repository.TryUpdate(bean));
            {
                Assert.IsTrue(repository.TryGet(bean.Key, out TBean bean0));
                Assert.Equals(bean, bean0);
            }
            Assert.IsTrue(repository.TryRemove(bean));
            Assert.IsFalse(repository.TryGet(bean.Key, out _));
            Assert.IsTrue(repository.TryAddOrUpdate(bean));
            {
                Assert.IsTrue(repository.TryGet(bean.Key, out TBean bean0));
                Assert.Equals(bean, bean0);
            }
            Assert.IsTrue(repository.TryAddOrUpdate(bean));
            {
                Assert.IsTrue(repository.TryGet(bean.Key, out TBean bean0));
                Assert.Equals(bean, bean0);
            }
            Assert.IsFalse(repository.TryAdd(bean));
        }
    }
}
