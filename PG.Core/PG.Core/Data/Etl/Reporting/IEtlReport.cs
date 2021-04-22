// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;

namespace PG.Core.Data.Etl.Reporting
{
    /// <summary>
    /// The base contract for ETL reports.
    /// </summary>
    public interface IEtlReport
    {
        /// <summary>
        /// The start time of the <see cref="IEtlReport"/>. Should always be auto-set by the .ctor
        /// </summary>
        DateTime StartTimeStamp { get; }
        /// <summary>
        /// The end time of the <see cref="IEtlReport"/>. Should always be set by the <see cref="FinalizeReport"/> method.
        /// </summary>
        DateTime EndTimeStamp { get; }
        /// <summary>
        /// The read-only list of <see cref="IEtlMessage"/>s generated during the ETL import.
        /// </summary>
        IEnumerable<IEtlMessage> EtlMessages { get; }
        /// <summary>
        /// The read-only list of <see cref="IEtlError"/>s generated during the ETL import.
        /// </summary>
        IEnumerable<IEtlError> EtlErrors { get; }
        /// <summary>
        /// Finalizes the report.
        /// </summary>
        void FinalizeReport();
        /// <summary>
        /// The report content as string.
        /// </summary>
        /// <returns></returns>
        string ToString();
    }
}
