using Timora.Data.Entities;

namespace Timora.Api.Services
{
    /// <summary>
    /// Service interface for document business logic operations.
    /// </summary>
    public interface IDocumentService
    {
        /// <summary>
        /// Gets all documents for a specific company.
        /// </summary>
        /// <param name="companyId">The company ID to filter by.</param>
        /// <returns>A collection of documents belonging to the specified company.</returns>
        Task<IEnumerable<Document>> GetDocumentsByCompanyIdAsync(int companyId);

        /// <summary>
        /// Gets a specific document by its ID.
        /// </summary>
        /// <param name="id">The document ID.</param>
        /// <returns>The document if found, otherwise null.</returns>
        Task<Document?> GetDocumentByIdAsync(int id);

        /// <summary>
        /// Creates a new document.
        /// </summary>
        /// <param name="document">The document to create.</param>
        /// <returns>The created document with navigation properties loaded.</returns>
        Task<Document> CreateDocumentAsync(Document document);

        /// <summary>
        /// Updates an existing document.
        /// </summary>
        /// <param name="id">The ID of the document to update.</param>
        /// <param name="document">The document data to apply.</param>
        /// <returns>The updated document if found, otherwise null.</returns>
        Task<Document?> UpdateDocumentAsync(int id, Document document);

        /// <summary>
        /// Deletes a document by its ID.
        /// </summary>
        /// <param name="id">The ID of the document to delete.</param>
        /// <returns>True if the document was deleted, false if not found.</returns>
        Task<bool> DeleteDocumentAsync(int id);
    }
}
