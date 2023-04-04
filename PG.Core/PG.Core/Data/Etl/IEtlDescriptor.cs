// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Core.Data.Etl.Reporting;

namespace PG.Core.Data.Etl
{
    /// <summary>
    /// The basic contract for an ETL descriptor
    /// </summary>
    public interface IEtlDescriptor
    {
        public IEtlReport Execute();

        public void ExtractPreProcess();

        public void Extract();

        public void ExtractPostProcess();

        public void TransformPreProcess();

        public void Transform();

        public void TransformPostProcess();

        public void LoadPreProcess();

        public void Load();

        public void LoadPostProcess();
    }
}
