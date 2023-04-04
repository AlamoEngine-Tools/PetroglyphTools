// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Core.Reporting.Export;
using System;
using System.Collections.Generic;

namespace PG.Core.Reporting
{
    public interface IReport
    {
        public DateTime ReportStartTime { get; }
        public DateTime ReportEndTime { get; }
        public TimeSpan ReportDuration { get; }
        public bool IsFinalized { get; }

        public void Export(ExportType exportType = ExportType.Full);

        internal IReportExportHandler ExportHandler { get; }
        internal IReadOnlyList<IErrorMessage> ErrorMessages { get; }
        internal IReadOnlyList<IMessage> Messages { get; }
    }
}
