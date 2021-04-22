// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace PG.Core.Data.Etl.Reporting
{
    /// <summary>
    /// The base contract for ETL errors.
    /// </summary>
    public interface IEtlMessage : IComparable<IEtlMessage>
    {
        /// <summary>
        /// The creation time of the <see cref="IEtlMessage"/>. Should always be auto-set by the .ctor
        /// </summary>
        DateTime TimeStamp { get; }
        /// <summary>
        /// The basic message.
        /// </summary>
        string Message { get; }
    }
}
