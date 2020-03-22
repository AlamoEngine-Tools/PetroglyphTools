using System.Collections.Concurrent;
using JetBrains.Annotations;

namespace PG.StarWarsGame.LSP.Server.Buffering
{
    internal class SimpleBufferManager : IBufferManager
    {
        [NotNull] private readonly ConcurrentDictionary<string, IDocumentBuffer> m_buffers =
            new ConcurrentDictionary<string, IDocumentBuffer>();

        public void UpdateBuffer(string documentPath, IDocumentBuffer buffer)
        {
            m_buffers.AddOrUpdate(documentPath, buffer, (k, v) => buffer);
        }

        public IDocumentBuffer GetBuffer(string documentPath)
        {
            return m_buffers.TryGetValue(documentPath, out IDocumentBuffer buffer) ? buffer : null;
        }
    }
}
