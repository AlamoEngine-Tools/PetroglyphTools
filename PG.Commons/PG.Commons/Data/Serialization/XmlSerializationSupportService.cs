// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;
using PG.Commons.Services;

namespace PG.Commons.Data.Serialization;

/// <inheritdoc cref="IXmlSerializationSupportService" />
public sealed class XmlSerializationSupportService : ServiceBase, IXmlSerializationSupportService
{
    /// <inheritdoc />
    public XmlSerializationSupportService(IServiceProvider services) : base(services)
    {
    }

    /// <inheritdoc />
    public string SerializeObject<T>(T serializableObject) where T : class
    {
        try
        {
            var xmlSerializer = new XmlSerializer(typeof(T));
            var stringWriter = new StringWriter();
            using var writer = XmlWriter.Create(stringWriter);
            xmlSerializer.Serialize(writer, serializableObject);
            return stringWriter.ToString();
        }
        catch (Exception e)
        {
            Logger.LogError("An exception occurred whilst serializing: {}", e);
        }

        return string.Empty;
    }

    /// <inheritdoc />
    public T? DeSerializeObjectFromDisc<T>(string filePath) where T : class
    {
        var fileInfo = FileSystem.FileInfo.New(filePath);
        if (!fileInfo.Exists) throw new FileNotFoundException("The file {} could not be found!", filePath);
        return DeSerializeObject<T>(FileSystem.File.ReadAllText(filePath));
    }

    /// <inheritdoc />
    public T? DeSerializeObject<T>(string xmlString) where T : class
    {
        if (string.IsNullOrEmpty(xmlString)) return null;
        var xmlSerializer = new XmlSerializer(typeof(T));
        var stringReader = new StringReader(xmlString);
        using var reader = XmlReader.Create(stringReader);
        var obj = xmlSerializer.Deserialize(reader);
        return obj as T;
    }

    /// <inheritdoc />
    public bool SerializeObjectAndStoreToDisc<T>(string filePath, T serializableObject, bool overwrite = false)
        where T : class
    {
        try
        {
            var fileInfo = FileSystem.FileInfo.New(filePath);
            if (fileInfo.Exists && !overwrite)
                throw new UnauthorizedAccessException(
                    $"The file already exists. Force overwrite with {nameof(overwrite)}=true.");
            fileInfo.Delete();

            var directoyInfo = fileInfo.Directory;
            if (directoyInfo is { Exists: false }) FileSystem.Directory.CreateDirectory(directoyInfo.FullName);

            FileSystem.File.WriteAllText(filePath, SerializeObject(serializableObject));
        }
        catch (Exception e)
        {
            Logger.LogError("An exception occurred whilst serializing: {}", e);
            return false;
        }

        return true;
    }
}
