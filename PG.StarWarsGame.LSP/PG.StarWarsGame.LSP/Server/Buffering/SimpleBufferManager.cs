using System.Collections.Concurrent;
using System.Diagnostics;
using JetBrains.Annotations;
using PG.Commons.Util;

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
            if (StringUtility.IsNullEmptyOrWhiteSpace(documentPath))
            {
                return null;
            }
            Debug.Assert(documentPath != null, nameof(documentPath) + " != null");
            return m_buffers.TryGetValue(documentPath, out IDocumentBuffer buffer) ? buffer : null;
        }
    }
}
