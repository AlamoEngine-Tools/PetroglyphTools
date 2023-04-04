// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Core.Exceptions;
using PG.Core.Reporting.Export;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PG.Core.Reporting
{
    public abstract class AbstractReport : IReport
    {
        public event EventHandler<IMessage> OnMessageAddedEvent;

        public event EventHandler<IErrorMessage> OnErrorMessageAddedEvent;

        public event EventHandler<bool> OnReportFinalizedEvent;

        private DateTime m_reportEndTime;
        private TimeSpan m_reportDuration = TimeSpan.Zero;
        private readonly List<IMessage> m_messages = new();
        private readonly IReportExportHandler m_exportHandler;

        public DateTime ReportStartTime { get; }
        public DateTime ReportEndTime => m_reportEndTime;
        public TimeSpan ReportDuration => CalculateReportDuration();

        IReadOnlyList<IErrorMessage> IReport.ErrorMessages =>
            (from m in m_messages
             where m.GetType().IsAssignableFrom(typeof(IErrorMessage))
             orderby m.CreatedTimestamp
             select m as IErrorMessage).ToList().AsReadOnly();

        IReadOnlyList<IMessage> IReport.Messages =>
            (from m in m_messages orderby m.CreatedTimestamp select m).ToList().AsReadOnly();

        public bool IsFinalized { get; private set; }

        IReportExportHandler IReport.ExportHandler => m_exportHandler;

        protected internal AbstractReport() : this(null)
        {
        }

        protected AbstractReport(IReportExportHandler exportHandler)
        {
            m_exportHandler = exportHandler ?? new NullReportExportHandler();
            ReportStartTime = DateTime.Now;
        }

        public virtual void FinalizeReport()
        {
            m_reportEndTime = DateTime.Now;
            m_reportDuration = CalculateReportDuration(true);
            IsFinalized = true;
            OnReportFinalized();
        }

        protected virtual void OnReportFinalized()
        {
            OnReportFinalizedEvent?.Invoke(this, true);
        }

        public virtual void AddMessage(IMessage m)
        {
            if (IsFinalized)
            {
                throw new ReportAlreadyFinalizedException(
                    $"The report has been finalized at {ReportEndTime:O} and cannot be expanded with new messages.");
            }

            if (m == null)
            {
                throw new ArgumentNullException(nameof(m));
            }

            m_messages.Add(m);
            OnMessageAdded(m);
        }

        protected virtual void OnMessageAdded(IMessage m)
        {
            if (m.GetType().IsInstanceOfType(typeof(IErrorMessage)))
            {
                OnErrorMessageAddedEvent?.Invoke(this, m as IErrorMessage);
            }

            OnMessageAddedEvent?.Invoke(this, m);
        }

        private TimeSpan CalculateReportDuration(bool isFinalizing = false)
        {
            if (isFinalizing)
            {
                return new TimeSpan(m_reportEndTime.Ticks - ReportStartTime.Ticks);
            }

            return IsFinalized ? m_reportDuration : new TimeSpan(DateTime.Now.Ticks - ReportStartTime.Ticks);
        }

        public virtual void Export(ExportType exportType = ExportType.Full)
        {
            m_exportHandler.CreateExport(this, exportType);
        }
    }
}
