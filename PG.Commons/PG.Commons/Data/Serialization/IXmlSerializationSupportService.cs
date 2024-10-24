namespace PG.Commons.Data.Serialization;

/// <summary>
///     Simple XML serialization helper service.
/// </summary>
public interface IXmlSerializationSupportService
{
    /// <summary>
    ///     Serializes an object to XML and stores it to the file path.
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="serializableObject"></param>
    /// <param name="overwrite"></param>
    /// <typeparam name="T"></typeparam>
    bool SerializeObjectAndStoreToDisc<T>(string filePath, T serializableObject, bool overwrite = true) where T : class;

    /// <summary>
    ///     Serializes the given object to an XML string.
    /// </summary>
    /// <param name="serializableObject"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    string SerializeObject<T>(T serializableObject) where T : class;

    /// <summary>
    ///     Deserializes an XML file read form disc to an object.
    /// </summary>
    /// <param name="filePath"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    T? DeSerializeObjectFromDisc<T>(string filePath) where T : class;

    /// <summary>
    ///     Deserializes an object from a XML string.
    /// </summary>
    /// <param name="xmlString"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    T? DeSerializeObject<T>(string xmlString) where T : class;
}
