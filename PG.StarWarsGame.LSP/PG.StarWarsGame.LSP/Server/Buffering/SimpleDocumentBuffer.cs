using System.Diagnostics;
using PG.Commons.Util;

namespace PG.StarWarsGame.LSP.Server.Buffering
{
    internal class SimpleDocumentBuffer : IDocumentBuffer
    { 
        private readonly string m_text;

        public SimpleDocumentBuffer(string text)
        {
            if (StringUtility.IsNullEmptyOrWhiteSpace(text))
            {
                text = string.Empty;
            }
            Debug.Assert(text != null, nameof(text) + " != null");
            this.m_text = text;
        }

        public int Length => m_text.Length;

        public char this[int index] => m_text[index];

        public string GetText(int start, int length)
        {
            return m_text.Substring(start, length);
        }

        public void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count)
        {
            if (null == destination)
            {
                destination = new char[0];
            }
            m_text.CopyTo(sourceIndex, destination, destinationIndex, count);
        }
    }
}
