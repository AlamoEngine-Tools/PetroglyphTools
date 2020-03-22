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
