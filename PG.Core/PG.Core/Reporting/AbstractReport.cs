// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;

namespace PG.Core.Reporting
{
    public abstract class AbstractReport : IReport
    {
        private bool m_isFinalized;
        private readonly List<IError> m_errors;
        private readonly List<IMessage> m_messages;
        public DateTime EventStart { get; }
        public DateTime EventEnd { get; private set; }
        public IReadOnlyList<IError> Errors => m_errors.AsReadOnly();
        public IReadOnlyList<IMessage> Messages  => m_messages.AsReadOnly();

        protected AbstractReport()
        {
            EventStart = DateTime.Now;
            m_errors = new List<IError>();
            m_messages = new List<IMessage>();
        }

        public virtual void Log(IError error)
        {
            m_errors.Add(error);
        }
        public virtual void Log(IMessage message)
        {
            m_messages.Add(message);
        }
        
        public void FinalizeReport()
        {
            if (m_isFinalized)
            {
                return;
            }

            m_isFinalized = true;
            EventEnd = DateTime.Now;
        }
    }
}
