// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;

namespace PG.Core.Data.Extract
{
    public interface IExtractService<TStage1>
    {
        IList<TStage1> Stage1Beans { get;  }
        public void Extract();
    }
}
