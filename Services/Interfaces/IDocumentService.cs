using CollegeWebSite.Domain.Entities;
using CollegeWebSite.Domain.Enums;

namespace CollegeWebSite.Services.Interfaces;

public interface IDocumentService
{
    Task<List<Document>> GetPublicDocumentsAsync();
    Task<List<Document>> GetDocumentsByTypeAsync(DocumentType type);
    Task<Document?> GetDocumentByIdAsync(long id);
    Task<Document> CreateDocumentAsync(Document document);
    Task<Document> UpdateDocumentAsync(Document document);
    Task<bool> DeleteDocumentAsync(long id);
}
