// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace PG.StarWarsGame.LSP.Server.Buffering
{
    public interface IBufferManager
    {
        void UpdateBuffer(string documentPath, IDocumentBuffer buffer);

        IDocumentBuffer GetBuffer(string key);
    }
}
