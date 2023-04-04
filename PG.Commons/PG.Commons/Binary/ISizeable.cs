// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace PG.Commons.Binary
{
    /// <summary>
    ///   Marks a given element as having a size. this can be used to iterate through binary streams with set file structures or header sizes.
    /// </summary>
    public interface ISizeable
    {

        /// <summary>Gets the chink size of the current binary.</summary>
        /// <value>The chunk size.</value>
        int Size { get; }
    }
}
