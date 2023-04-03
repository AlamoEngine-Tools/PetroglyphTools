// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace PG.Core.Reporting.Export
{
/// <summary>
    /// Base implementation of the <see cref="IReportExportHandler"/> contract.
    /// Handles the basic error checking and delegation. All implementations of the <see cref="IReportExportHandler"/>
    /// interface should inherit from this base class.
    /// </summary>
    public abstract class AbstractReportExportHandler : IReportExportHandler
    {
        public void CreateExport(IReport report, ExportType exportType = ExportType.Full)
        {
            if (report == null)
            {
                throw new ArgumentNullException(nameof(report));
            }

            if (!report.IsFinalized)
            {
                throw new ArgumentException(
                    $"The provided {nameof(IReport)} {nameof(report)} is not finalized and cannot be exported.");
            }

            switch (exportType)
            {
                case ExportType.Full:
                    CreateFullExport(report);
                    break;
                case ExportType.MessagesOnly:
                    CreateMessageExport(report);
                    break;
                case ExportType.ErrorsOnly:
                    CreateErrorExport(report);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(exportType), exportType, null);
            }
        }

        /// <summary>
        /// Exports all relevant <see cref="IReport"/> data as well as all <see cref="IErrorMessage"/>s.
        /// </summary>
        /// <param name="report"></param>
        protected abstract void CreateErrorExport(IReport report);

        /// <summary>
        /// Exports all relevant <see cref="IReport"/> data as well as all <see cref="IMessage"/>s that are <b>NOT</b>
        /// <see cref="IErrorMessage"/>s.
        /// </summary>
        /// <param name="report"></param>
        protected abstract void CreateMessageExport(IReport report);
        /// <summary>
        /// Exports all relevant <see cref="IReport"/> data as well as all <see cref="IMessage"/>s, thereby creating a
        /// full export of the provided <see cref="IReport"/>.
        /// </summary>
        /// <remarks>This is the default behaviour when calling <see cref="CreateExport"/> without explicit
        /// <see cref="ExportType"/>.</remarks>
        /// <param name="report"></param>
        protected abstract void CreateFullExport(IReport report);
    }
}
