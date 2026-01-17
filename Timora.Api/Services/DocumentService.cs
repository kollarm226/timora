using Timora.Api.Repositories;
using Timora.Data.Entities;

namespace Timora.Api.Services
{
    /// <summary>
    /// Service implementation for document business logic operations.
    /// </summary>
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _documentRepository;

        /// <summary>
        /// Initializes a new instance of the DocumentService.
        /// </summary>
        /// <param name="documentRepository">The document repository.</param>
        public DocumentService(IDocumentRepository documentRepository)
        {
            _documentRepository = documentRepository;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Document>> GetDocumentsByCompanyIdAsync(int companyId)
        {
            return await _documentRepository.GetDocumentsByCompanyIdAsync(companyId);
        }

        /// <inheritdoc />
        public async Task<Document?> GetDocumentByIdAsync(int id)
        {
            return await _documentRepository.GetDocumentByIdAsync(id);
        }

        /// <inheritdoc />
        public async Task<Document> CreateDocumentAsync(Document document)
        {
            return await _documentRepository.CreateDocumentAsync(document);
        }

        /// <inheritdoc />
        public async Task<Document?> UpdateDocumentAsync(int id, Document document)
        {
            return await _documentRepository.UpdateDocumentAsync(id, document);
        }

        /// <inheritdoc />
        public async Task<bool> DeleteDocumentAsync(int id)
        {
            return await _documentRepository.DeleteDocumentAsync(id);
        }
    }
}
