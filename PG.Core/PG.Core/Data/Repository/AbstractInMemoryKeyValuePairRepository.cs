// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using PG.Core.Data.Bean;
using PG.Core.Data.Key;

namespace PG.Core.Data.Repository
{
    /// <summary>
    /// Abstract base implementation of an in-memory <see cref="IRepository{TKey,TBean}"/>  
    /// </summary>
    /// <typeparam name="TKey">The identifying <see cref="IKey"/>.</typeparam>
    /// <typeparam name="TBean">The contained <see cref="IBean{TKey}"/></typeparam>
    public abstract class AbstractInMemoryKeyValuePairRepository<TKey, TBean> : IRepository<TKey, TBean>, IDisposable
        where TKey : IKey where TBean : IBean<TKey>
    {
        private bool m_isDisposed;
        private IDictionary<TKey, TBean> m_beans = new ConcurrentDictionary<TKey, TBean>();

        /// <summary>
        /// The inheritable repository content holder.
        /// </summary>
        protected internal virtual IDictionary<TKey, TBean> Beans => m_beans;

        public virtual bool TryAdd(TBean bean)
        {
            if (null == bean || Beans.ContainsKey(bean.Key))
            {
                return false;
            }

            Beans.Add(bean.Key, bean);
            return true;
        }

        public virtual bool TryUpdate(TBean bean)
        {
            if (null == bean || !Beans.ContainsKey(bean.Key))
            {
                return false;
            }

            Beans[bean.Key] = bean;
            return true;
        }

        public virtual bool TryAddOrUpdate(TBean bean)
        {
            if (null == bean)
            {
                return false;
            }

            return Beans.ContainsKey(bean.Key) ? TryUpdate(bean) : TryAdd(bean);
        }

        public virtual bool TryRemove(TBean bean)
        {
            return null != bean && TryRemove(bean.Key);
        }

        public virtual bool TryRemove(TKey key)
        {
            if (null == key || !Beans.ContainsKey(key))
            {
                return false;
            }

            return Beans.Remove(key);
        }

        public virtual bool TryGet(TKey key, out TBean bean)
        {
            bean = default;
            if (null == key || !Beans.ContainsKey(key))
            {
                return false;
            }

            return Beans.TryGetValue(key, out bean);
        }

        #region IDisposable Pattern

        protected internal virtual void DisposeInternal(bool disposing)
        {
            if (!m_isDisposed)
            {
                if (disposing)
                {
                    DisposePrivateObjectReferencesInternal();
                }
            }

            ReleaseUnmanagedResourcesInternal();
            NullLargeFieldsInternal();
            m_isDisposed = true;
        }

        protected internal void DisposePrivateObjectReferencesInternal()
        {
            // NOP
        }

        protected internal void ReleaseUnmanagedResourcesInternal()
        {
            // NOP
        }

        protected internal void NullLargeFieldsInternal()
        {
            m_beans = null;
        }

        public void Dispose()
        {
            DisposeInternal(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
