using System.IO;
using System.IO.Abstractions;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace PG.Commons.Util
{
    public static class XmlUtility
    {
        public static IFileSystem FileSystem { get; set; }

        static XmlUtility()
        {
            FileSystem = new FileSystem();
        }

        public static T ReadAndValidateXmlFile<T>(string embeddedXsdFileName, string xmlFilePath)
        {
            T xmlData;
            XmlSerializer xsdSchemaSerializer = new XmlSerializer(typeof(XmlSchema));
            XmlSerializer xmlDataSerializer = new XmlSerializer(typeof(T));

            XmlSchemaSet schemas = new XmlSchemaSet();
            XmlSchema schema;
            using (Stream xsdStream = EmbeddedResourceUtility.GetResourceAsStreamByFileName(embeddedXsdFileName))
            {
                schema = (XmlSchema) xsdSchemaSerializer.Deserialize(xsdStream);
            }

            schemas.Add(schema);
            XmlReaderSettings settings = new XmlReaderSettings
            {
                Schemas = schemas,
                ValidationType = ValidationType.Schema,
                ValidationFlags = XmlSchemaValidationFlags.ProcessIdentityConstraints |
                                  XmlSchemaValidationFlags.ReportValidationWarnings |
                                  XmlSchemaValidationFlags.ProcessInlineSchema |
                                  XmlSchemaValidationFlags.ProcessSchemaLocation
            };

            settings.ValidationEventHandler +=
                (sender, arguments) => throw new XmlSchemaValidationException(arguments.Message);

            using (Stream xmlStream = FileSystem.File.OpenRead(xmlFilePath))
            using (XmlReader reader = XmlReader.Create(xmlStream, settings))
            {
                xmlData = (T) xmlDataSerializer.Deserialize(reader);
            }

            if (xmlData != null)
            {
                return xmlData;
            }

            throw new XmlSchemaException("The xml could not be deserialized.");
        }
        
        public static void WriteXmlFile<T>(string filePath, T content, bool prettyPrint = true)
        {
            XmlSerializer xmlDataSerializer = new XmlSerializer(typeof(T));
            using XmlWriter xmlWriter = XmlWriter.Create(FileSystem.File.OpenWrite(filePath), new XmlWriterSettings()
            {
                Encoding = Encoding.UTF8,
                Indent = prettyPrint,
                CheckCharacters = true,
                OmitXmlDeclaration = true
            });
            xmlDataSerializer.Serialize(xmlWriter, content);
        }
    }
}