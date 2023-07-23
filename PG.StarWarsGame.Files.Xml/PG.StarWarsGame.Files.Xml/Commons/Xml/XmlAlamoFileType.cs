// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Commons.Data.Files;

namespace PG.StarWarsGame.Files.Xml.Commons.Xml;

internal class XmlAlamoFileType : IAlamoFileType
{
    private const FileType FILE_TYPE = FileType.Text;
    private const string FILE_EXTENSION = "xml";

    public FileType Type => FILE_TYPE;
    public string FileExtension => FILE_EXTENSION;
}