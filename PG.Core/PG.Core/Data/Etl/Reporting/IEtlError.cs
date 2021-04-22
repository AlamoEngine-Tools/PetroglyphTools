// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace PG.Core.Data.Etl.Reporting
{
    /// <summary>
    /// The base contract for ETL errors.
    /// </summary>
    public interface IEtlError : IComparable<IEtlError>
    {
        /// <summary>
        /// The creation time of the <see cref="IEtlError"/>. Should always be auto-set by the .ctor
        /// </summary>
        DateTime TimeStamp { get; }
        /// <summary>
        /// The basic error message.
        /// </summary>
        string Message { get; }
        /// <summary>
        /// The optional exception that caused the error report.
        /// </summary>
        Exception Exception { get; }
    }
}
