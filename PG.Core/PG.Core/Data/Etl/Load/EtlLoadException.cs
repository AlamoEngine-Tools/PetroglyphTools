// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Runtime.Serialization;

namespace PG.Core.Data.Etl.Load
{
    [Serializable]
    public sealed class EtlLoadException : Exception
    {
        public EtlLoadException()
        {
        }

        public EtlLoadException(string message) : base(message)
        {
        }

        public EtlLoadException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public EtlLoadException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        
    }
}
