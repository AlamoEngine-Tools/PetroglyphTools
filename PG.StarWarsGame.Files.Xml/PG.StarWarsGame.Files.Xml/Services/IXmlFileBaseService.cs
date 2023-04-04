// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Xml.Linq;

namespace PG.StarWarsGame.Services
{
    public interface IXmlFileBaseService
    {
        XDocument LoadFlat(string filePath);
        void StoreFlat(XDocument xDocument, string filePath);
    }
}
