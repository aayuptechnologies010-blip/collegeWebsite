using CollegeWebSite.Data;
using CollegeWebSite.Domain.Entities;
using CollegeWebSite.Domain.Enums;
using CollegeWebSite.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CollegeWebSite.Services.Implementation;

public class DocumentService : IDocumentService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public DocumentService(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<Document>> GetPublicDocumentsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var now = DateTime.UtcNow;
        
        return await context.Documents
            .Include(d => d.Media)
            .Where(d => d.IsPublic && 
                       d.IsActive &&
                       (d.ValidFrom == null || d.ValidFrom <= now) &&
                       (d.ValidTo == null || d.ValidTo >= now))
            .OrderByDescending(d => d.CreatedOn)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Document>> GetDocumentsByTypeAsync(DocumentType type)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var now = DateTime.UtcNow;
        
        return await context.Documents
            .Include(d => d.Media)
            .Where(d => d.Type == type &&
                       d.IsPublic &&
                       d.IsActive &&
                       (d.ValidFrom == null || d.ValidFrom <= now) &&
                       (d.ValidTo == null || d.ValidTo >= now))
            .OrderByDescending(d => d.CreatedOn)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Document?> GetDocumentByIdAsync(long id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Documents
            .Include(d => d.Media)
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<Document> CreateDocumentAsync(Document document)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        context.Documents.Add(document);
        await context.SaveChangesAsync();
        return document;
    }

    public async Task<Document> UpdateDocumentAsync(Document document)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        context.Documents.Update(document);
        await context.SaveChangesAsync();
        return document;
    }

    public async Task<bool> DeleteDocumentAsync(long id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var document = await context.Documents.FindAsync(id);
        if (document != null)
        {
            context.Documents.Remove(document);
            return await context.SaveChangesAsync() > 0;
        }
        return false;
    }
}
