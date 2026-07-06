using CollegeWebSite.Data;
using CollegeWebSite.Domain.Entities;
using CollegeWebSite.Domain.Enums;
using CollegeWebSite.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CollegeWebSite.Services.Implementation;

public class InquiryService : IInquiryService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly ILogger<InquiryService> _logger;

    public InquiryService(IDbContextFactory<ApplicationDbContext> contextFactory, ILogger<InquiryService> logger)
    {
        _contextFactory = contextFactory;
        _logger = logger;
    }

    public async Task<Inquiry> CreateInquiryAsync(Inquiry inquiry)
    {
        _logger.LogInformation("Institutional Record: New Inquiry received from {Name} ({Email}) regarding: {Subject}", inquiry.Name, inquiry.Email, inquiry.Subject);
        using var context = await _contextFactory.CreateDbContextAsync();
        inquiry.SubmittedOn = DateTime.UtcNow;
        context.Inquiries.Add(inquiry);
        await context.SaveChangesAsync();
        return inquiry;
    }

    public async Task<List<Inquiry>> GetAllInquiriesAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Inquiries
            .OrderByDescending(i => i.SubmittedOn)
            .ToListAsync();
    }

    public async Task<Inquiry> GetInquiryByIdAsync(long id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Inquiries.FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<bool> UpdateInquiryStatusAsync(long id, InquiryStatus status, string? notes = null)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var inquiry = await context.Inquiries.FirstOrDefaultAsync(i => i.Id == id);
        if (inquiry == null) return false;

        inquiry.Status = status;
        if (!string.IsNullOrEmpty(notes))
        {
            inquiry.Notes = notes;
        }

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<int> GetTotalInquiriesCountAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Inquiries.CountAsync();
    }

    public async Task<List<Inquiry>> GetRecentInquiriesAsync(int count)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Inquiries
            .OrderByDescending(i => i.SubmittedOn)
            .Take(count)
            .ToListAsync();
    }

    public async Task<bool> DeleteInquiryAsync(long id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var inquiry = await context.Inquiries.FindAsync(id);
        if (inquiry == null) return false;
        inquiry.DeletedOn = DateTime.UtcNow;
        context.Inquiries.Update(inquiry);
        return await context.SaveChangesAsync() > 0;
    }
}
