using Microsoft.EntityFrameworkCore;
using CollegeWebSite.Domain.Entities;

namespace CollegeWebSite.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<PageTemplate> PageTemplates { get; set; }
    public DbSet<PageContent> PageContents { get; set; }
    public DbSet<PageSeo> PageSeos { get; set; }
    public DbSet<Media> Media { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<Faculty> Faculty { get; set; }
    public DbSet<GalleryCategory> GalleryCategories { get; set; }
    public DbSet<GalleryImage> GalleryImages { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<Inquiry> Inquiries { get; set; }
    public DbSet<PlacementRecord> PlacementRecords { get; set; }
    public DbSet<StudentResult> StudentResults { get; set; }
    public DbSet<CampusNotice> CampusNotices { get; set; }
    public DbSet<Syllabus> Syllabuses { get; set; }
    public DbSet<ApplicationSettings> ApplicationSettings { get; set; }
    public DbSet<HeroContent> HeroSectionContent { get; set; }
    public DbSet<AnnouncementPopUp> AnnouncementPopUps { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User-Role Many-to-Many relationship
        modelBuilder.Entity<UserRole>()
            .HasKey(ur => ur.Id);

        modelBuilder.Entity<UserRole>()
            .HasIndex(ur => new { ur.UserId, ur.RoleId })
            .IsUnique();

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        // PageContent relationships
        modelBuilder.Entity<PageContent>()
            .HasIndex(p => p.Slug)
            .IsUnique();

        modelBuilder.Entity<PageContent>()
            .HasOne(p => p.Template)
            .WithMany(t => t.Pages)
            .HasForeignKey(p => p.TemplateId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<PageContent>()
            .HasOne(p => p.Seo)
            .WithOne(s => s.PageContent)
            .HasForeignKey<PageSeo>(s => s.Id)
            .OnDelete(DeleteBehavior.Cascade);

        // Document relationships
        modelBuilder.Entity<Document>()
            .HasOne(d => d.Media)
            .WithMany()
            .HasForeignKey(d => d.MediaId)
            .OnDelete(DeleteBehavior.Restrict);

        // Faculty relationships
        modelBuilder.Entity<Faculty>()
            .HasOne(f => f.Photo)
            .WithMany()
            .HasForeignKey(f => f.PhotoMediaId)
            .OnDelete(DeleteBehavior.SetNull);

        // GalleryImage relationships
        modelBuilder.Entity<GalleryImage>()
            .HasOne(gi => gi.Media)
            .WithMany()
            .HasForeignKey(gi => gi.MediaId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<GalleryImage>()
            .HasOne(gi => gi.Category)
            .WithMany(gc => gc.Images)
            .HasForeignKey(gi => gi.GalleryCategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        // Event relationships
        modelBuilder.Entity<Event>()
            .HasOne(e => e.Banner)
            .WithMany()
            .HasForeignKey(e => e.BannerMediaId)
            .OnDelete(DeleteBehavior.SetNull);

        // Inquiry relationships
        modelBuilder.Entity<Inquiry>()
            .HasOne(i => i.AssignedToUser)
            .WithMany()
            .HasForeignKey(i => i.AssignedToUserId)
            .OnDelete(DeleteBehavior.SetNull);

        // PlacementRecord relationships
        modelBuilder.Entity<PlacementRecord>()
            .HasOne(p => p.StudentPhoto)
            .WithMany()
            .HasForeignKey(p => p.StudentPhotoId)
            .OnDelete(DeleteBehavior.SetNull);

        // Syllabus relationships
        modelBuilder.Entity<Syllabus>()
            .HasOne(s => s.DocumentMedia)
            .WithMany()
            .HasForeignKey(s => s.DocumentMediaId)
            .OnDelete(DeleteBehavior.SetNull);

        // ApplicationSettings unique key
        modelBuilder.Entity<ApplicationSettings>()
            .HasIndex(a => a.Key)
            .IsUnique();

        // Soft delete filter for AuditableEntity
        modelBuilder.Entity<User>().HasQueryFilter(u => u.DeletedOn == null);
        modelBuilder.Entity<Role>().HasQueryFilter(r => r.DeletedOn == null);
        modelBuilder.Entity<PageTemplate>().HasQueryFilter(pt => pt.DeletedOn == null);
        modelBuilder.Entity<PageContent>().HasQueryFilter(pc => pc.DeletedOn == null);
        modelBuilder.Entity<Media>().HasQueryFilter(m => m.DeletedOn == null);
        modelBuilder.Entity<Document>().HasQueryFilter(d => d.DeletedOn == null);
        modelBuilder.Entity<Faculty>().HasQueryFilter(f => f.DeletedOn == null);
        modelBuilder.Entity<GalleryCategory>().HasQueryFilter(gc => gc.DeletedOn == null);
        modelBuilder.Entity<GalleryImage>().HasQueryFilter(gi => gi.DeletedOn == null);
        modelBuilder.Entity<Event>().HasQueryFilter(e => e.DeletedOn == null);
        modelBuilder.Entity<PlacementRecord>().HasQueryFilter(p => p.DeletedOn == null);
        modelBuilder.Entity<StudentResult>().HasQueryFilter(sr => sr.DeletedOn == null);
        modelBuilder.Entity<CampusNotice>().HasQueryFilter(cn => cn.DeletedOn == null);
        modelBuilder.Entity<Syllabus>().HasQueryFilter(s => s.DeletedOn == null);
        modelBuilder.Entity<Inquiry>().HasQueryFilter(i => i.DeletedOn == null);
        modelBuilder.Entity<HeroContent>().HasQueryFilter(h => h.DeletedOn == null);
        modelBuilder.Entity<AnnouncementPopUp>().HasQueryFilter(p => p.DeletedOn == null);

        // Matching filters for required ends to avoid validation warnings
        modelBuilder.Entity<PageSeo>().HasQueryFilter(s => s.PageContent.DeletedOn == null);
        modelBuilder.Entity<UserRole>().HasQueryFilter(ur => ur.Role.DeletedOn == null && ur.User.DeletedOn == null);
    }
}
