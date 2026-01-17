using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Timora.Data.Data;
using Timora.Data.Entities;

namespace Timora.Api.Repositories
{
    /// <summary>
    /// Repository implementation for document data access operations.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class DocumentRepository : IDocumentRepository
    {
        private readonly TimoraDbContext _context;

        /// <summary>
        /// Initializes a new instance of the DocumentRepository.
        /// </summary>
        /// <param name="context">The database context.</param>
        public DocumentRepository(TimoraDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Document>> GetDocumentsByCompanyIdAsync(int companyId)
        {
            return await _context.Documents
                .Include(d => d.Company)
                .Where(d => d.CompanyId == companyId)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<Document?> GetDocumentByIdAsync(int id)
        {
            return await _context.Documents
                .Include(d => d.Company)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        /// <inheritdoc />
        public async Task<Document> CreateDocumentAsync(Document document)
        {
            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            // Reload with navigation properties
            return await _context.Documents
                .Include(d => d.Company)
                .FirstAsync(d => d.Id == document.Id);
        }

        /// <inheritdoc />
        public async Task<Document?> UpdateDocumentAsync(int id, Document document)
        {
            var existingDocument = await _context.Documents.FindAsync(id);
            if (existingDocument == null)
            {
                return null;
            }

            // Apply partial updates
            if (!string.IsNullOrEmpty(document.Title))
            {
                existingDocument.Title = document.Title;
            }
            if (!string.IsNullOrEmpty(document.Description))
            {
                existingDocument.Description = document.Description;
            }
            if (!string.IsNullOrEmpty(document.FileUrl))
            {
                existingDocument.FileUrl = document.FileUrl;
            }

            await _context.SaveChangesAsync();

            return await _context.Documents
                .Include(d => d.Company)
                .FirstAsync(d => d.Id == id);
        }

        /// <inheritdoc />
        public async Task<bool> DeleteDocumentAsync(int id)
        {
            var document = await _context.Documents.FindAsync(id);
            if (document == null)
            {
                return false;
            }

            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
