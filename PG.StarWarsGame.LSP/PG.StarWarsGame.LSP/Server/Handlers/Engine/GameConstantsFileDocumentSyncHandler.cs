using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using PG.StarWarsGame.LSP.Server.Buffering;

namespace PG.StarWarsGame.LSP.Server.Handlers.Engine
{
    internal class GameConstantsFileDocumentSyncHandler : ITextDocumentSyncHandler
    {
        [NotNull] private readonly IBufferManager m_bufferManager;

        private readonly DocumentSelector m_documentSelector = new DocumentSelector(
            new DocumentFilter {Pattern = "**/GameConstantsFile.xml"}
        );

        [NotNull] private readonly ILanguageServer m_router;
        private SynchronizationCapability _capability;

        public GameConstantsFileDocumentSyncHandler(ILanguageServer router, IBufferManager bufferManager)
        {
            m_router = router ?? throw new ArgumentNullException($"The argument {nameof(router)} may not be null.");
            m_bufferManager = bufferManager ??
                              throw new ArgumentNullException($"The argument {nameof(bufferManager)} may not be null.");
        }
        //TODO: Update to TextDocumentSyncKind.Incremental
        public TextDocumentSyncKind Change { get; } = TextDocumentSyncKind.Full;

        #region ITextDocumentSyncHandler Implementation

        public Task<Unit> Handle(DidChangeTextDocumentParams request, CancellationToken cancellationToken)
        {
            if (request?.TextDocument == null)
            {
                return Unit.Task;
            }

            if (null == request.TextDocument.Uri)
            {
                return Unit.Task;
            }

            string documentPath = request.TextDocument.Uri.ToString();
            if (null == request.ContentChanges)
            {
                return Unit.Task;
            }

            if (null == request.ContentChanges.FirstOrDefault())
            {
                return Unit.Task;
            }

            string text = request.ContentChanges.FirstOrDefault()?.Text;
            m_bufferManager.UpdateBuffer(documentPath, new SimpleDocumentBuffer(text));
            m_router.Window.LogInfo(
                LocalizableTexts.ResourceManager.GetString("GameConstantsFileDocumentSyncHandler_Handle_LogInfo") +
                ": " + documentPath + "\n" + text);
            return Unit.Task;
        }

        TextDocumentChangeRegistrationOptions IRegistration<TextDocumentChangeRegistrationOptions>.
            GetRegistrationOptions()
        {
            return new TextDocumentChangeRegistrationOptions()
            {
                DocumentSelector = m_documentSelector, SyncKind = Change
            };
        }

        public void SetCapability(SynchronizationCapability capability)
        {
            _capability = capability;
        }

        public Task<Unit> Handle(DidOpenTextDocumentParams request, CancellationToken cancellationToken)
        {
            if (request?.TextDocument == null)
            {
                return Unit.Task;
            }

            if (null == request.TextDocument.Uri)
            {
                return Unit.Task;
            }

            m_bufferManager.UpdateBuffer(request.TextDocument.Uri.ToString(),
                new SimpleDocumentBuffer(request.TextDocument.Text));
            return Unit.Task;
        }

        TextDocumentRegistrationOptions IRegistration<TextDocumentRegistrationOptions>.GetRegistrationOptions()
        {
            return new TextDocumentRegistrationOptions() {DocumentSelector = m_documentSelector,};
        }

        public Task<Unit> Handle(DidCloseTextDocumentParams request, CancellationToken cancellationToken)
        {
            return Unit.Task;
        }

        public Task<Unit> Handle(DidSaveTextDocumentParams request, CancellationToken cancellationToken)
        {
            return Unit.Task;
        }

        TextDocumentSaveRegistrationOptions IRegistration<TextDocumentSaveRegistrationOptions>.GetRegistrationOptions()
        {
            return new TextDocumentSaveRegistrationOptions()
            {
                DocumentSelector = m_documentSelector, IncludeText = true
            };
        }

        public TextDocumentAttributes GetTextDocumentAttributes(DocumentUri uri)
        {
            return new TextDocumentAttributes(uri, "xml");
            // TODO: Update to return new TextDocumentAttributes(uri, "eaw.gameconstantsfile.xml");
        }

        #endregion
    }
}
