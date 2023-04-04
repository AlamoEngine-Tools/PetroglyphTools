// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace PG.Core.Reporting.Export
{
    /// <summary>
    /// Basic contract for an export handler capable of exporting an object implementing the <see cref="IReport"/>
    /// interface.
    /// </summary>
    public interface IReportExportHandler
    {
        /// <summary>
        /// Creates an export for the provided <see cref="IReport"/>.
        /// </summary>
        /// <param name="report">The <see cref="IReport"/> to export. Usually the parent of the handler object.</param>
        /// <param name="exportType">Optional. Defaults to <see cref="ExportType.Full"/></param>
        void CreateExport(IReport report, ExportType exportType = ExportType.Full);
    }
}
