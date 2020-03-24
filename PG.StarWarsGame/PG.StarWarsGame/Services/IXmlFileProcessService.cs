using PG.Commons.Data.Holder;
using PG.Commons.Xml;

namespace PG.StarWarsGame.Services
{
    public interface IXmlFileProcessService<TXmlFileDefinition, TContent> where TXmlFileDefinition : IXmlFileDefinition
    {
        IXmlDocumentHolder<TContent> Load(string filePath);
        void Store(IXmlDocumentHolder<TContent> xmlDocumentHolder);
        void Store(IXmlDocumentHolder<TContent> xmlDocumentHolder, string filePath);
    }
}
