// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace PG.Core.Reporting.Export
{
    /// <summary>
    /// Default <see cref="IReportExportHandler"/> that is used by an <see cref="AbstractReport"/> if no explicit
    /// <see cref="IReportExportHandler"/> is provided.
    /// </summary>
    public sealed class NullReportExportHandler : AbstractReportExportHandler
    {
        protected override void CreateErrorExport(IReport report)
        {
            // NOP
        }

        protected override void CreateMessageExport(IReport report)
        {
            // NOP
        }

        protected override void CreateFullExport(IReport report)
        {
            // NOP
        }
    }
}
