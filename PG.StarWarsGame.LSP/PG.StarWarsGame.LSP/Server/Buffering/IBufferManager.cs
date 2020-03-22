namespace PG.StarWarsGame.LSP.Server.Buffering
{
    public interface IBufferManager
    {
        void UpdateBuffer(string documentPath, IDocumentBuffer buffer);
        IDocumentBuffer GetBuffer(string key);
    }
}