using PG.Commons.Data.Holder;
using PG.Commons.Xml;

namespace PG.StarWarsGame.Files.Xml.Services
{
    public interface IXmlFileProcessService<TXmlFileDefinition, TContent> where TXmlFileDefinition : IXmlFileDescriptor
    {
        IXmlDocumentHolder<TContent> Load(string filePath);
        void Store(IXmlDocumentHolder<TContent> xmlDocumentHolder);
        void Store(IXmlDocumentHolder<TContent> xmlDocumentHolder, string filePath);
    }
}
