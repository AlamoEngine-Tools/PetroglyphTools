// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace PG.StarWarsGame.LSP.Server.Buffering
{
    public interface IDocumentBuffer
    {
        int Length { get; }
        char this[int index] { get; }

        string GetText(int start, int length);

        void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count);
    }
}
