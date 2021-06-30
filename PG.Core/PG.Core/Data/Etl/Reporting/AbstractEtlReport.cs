// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PG.Core.Data.Etl.Reporting
{
    /// <summary>
    /// Base <see cref="IEtlReport"/> implementation.
    /// </summary>
    public abstract class AbstractEtlReport : IEtlReport
    {
        private readonly List<IEtlMessage> m_etlMessagesInternal = new List<IEtlMessage>();
        private readonly List<IEtlError> m_etlErrorsInternal = new List<IEtlError>();

        /// <summary>
        /// The log timestamp format.
        /// </summary>
        /// <returns></returns>
        protected internal virtual string ConfiguredTimeStampFormat => "O";

        /// <summary>
        /// set to true by the default <see cref="FinalizeReport"/> implementation.
        /// </summary>
        protected internal bool IsFinalized { get; private set; }

        protected internal List<IEtlMessage> EtlMessagesInternal
        {
            get
            {
                m_etlMessagesInternal.Sort();
                return m_etlMessagesInternal;
            }
        }

        protected internal List<IEtlError> EtlErrorsInternal
        {
            get
            {
                m_etlErrorsInternal.Sort();
                return m_etlErrorsInternal;
            }
        }


        public DateTime StartTimeStamp { get; } = DateTime.Now;
        public DateTime EndTimeStamp { get; private set; }
        public IEnumerable<IEtlMessage> EtlMessages => EtlMessagesInternal.AsReadOnly();
        public IEnumerable<IEtlError> EtlErrors => EtlErrorsInternal.AsReadOnly();

        public void FinalizeReport()
        {
            IsFinalized = true;
            EndTimeStamp = DateTime.Now;
            FinalizeReportInternal();
        }

        /// <summary>
        /// Called by <see cref="FinalizeReport"/>.
        /// Override this method to add to the base <see cref="FinalizeReport"/> call.
        /// </summary>
        protected internal virtual void FinalizeReportInternal()
        {
            // NOP
        }

        /// <summary>
        /// The internal <see cref="ToString"/> representation. Override this method, instead of <see cref="ToString"/>
        /// </summary>
        /// <returns></returns>
        protected internal virtual string ToStringInternal()
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (!IsFinalized)
            {
                stringBuilder.AppendLine("The report has not yet been finalized.");
                return stringBuilder.ToString();
            }
            stringBuilder.AppendLine(
                $"ETL Report - Start: {StartTimeStamp.ToString(ConfiguredTimeStampFormat)} - End: {EndTimeStamp.ToString(ConfiguredTimeStampFormat)}");

            stringBuilder.AppendLine("ETL Errors");
            foreach (IEtlError etlError in EtlErrors)
            {
                stringBuilder.AppendLine(
                    $"[{etlError.TimeStamp.ToString(ConfiguredTimeStampFormat)}] - {etlError.Message}");
                if (etlError.Exception != null)
                {
                    stringBuilder.AppendLine(etlError.Exception.ToString());
                }
            }

            if (!EtlMessages.Any())
            {
                return stringBuilder.ToString();
            }

            foreach (IEtlMessage etlMessage in EtlMessages)
            {
                stringBuilder.AppendLine(
                    $"[{etlMessage.TimeStamp.ToString(ConfiguredTimeStampFormat)}] - {etlMessage.Message}");
            }

            return stringBuilder.ToString();
        }

        public new string ToString()
        {
            return ToStringInternal();
        }
    }
}
