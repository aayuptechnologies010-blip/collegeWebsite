using CollegeWebSite.Components;
using CollegeWebSite.Data;
using CollegeWebSite.Infrastructure;
using Microsoft.EntityFrameworkCore;
using CollegeWebSite.Domain.Settings;
using Serilog;
using Microsoft.AspNetCore.Components;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(Path.Combine("Logs", "System-.log"), 
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(7);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddScoped(sp => {
    var nav = sp.GetRequiredService<NavigationManager>();
    return new HttpClient { BaseAddress = new Uri(nav.BaseUri) };
});

// Add Entity Framework Core with Database Provider support
var databaseProvider = builder.Configuration["DatabaseProvider"];
string? connectionString = null;

// Using InMemory Database for local UI testing
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("CollegeDbLocal"));
    
// Also register regular DbContext for scoped services if needed
builder.Services.AddScoped<ApplicationDbContext>(p => 
    p.GetRequiredService<IDbContextFactory<ApplicationDbContext>>().CreateDbContext());

/*
// MySQL support re-enabled with Pomelo.EntityFrameworkCore.MySql
if (string.Equals(databaseProvider, "MySQL", StringComparison.OrdinalIgnoreCase))
{
    connectionString = builder.Configuration.GetConnectionString("MySQL");
    if (string.IsNullOrEmpty(connectionString)) throw new InvalidOperationException("Connection string 'MySQL' not found.");
    
    // Using AddDbContextFactory for Blazor Server thread safety
    builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
        options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 35))));
        
    // Also register regular DbContext for scoped services if needed
    builder.Services.AddScoped<ApplicationDbContext>(p => 
        p.GetRequiredService<IDbContextFactory<ApplicationDbContext>>().CreateDbContext());
}
else // Default to PostgreSQL
{
    connectionString = builder.Configuration.GetConnectionString("PostgreSQL");
    // Fallback to "DefaultConnection" if PostgreSQL specific not found
    if (string.IsNullOrEmpty(connectionString)) 
        connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        
    if (string.IsNullOrEmpty(connectionString)) throw new InvalidOperationException("Connection string 'PostgreSQL' or 'DefaultConnection' not found.");

    // Using AddDbContextFactory for Blazor Server thread safety
    builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
        options.UseNpgsql(connectionString));
        
    // Also register regular DbContext for scoped services if needed
    builder.Services.AddScoped<ApplicationDbContext>(p => 
        p.GetRequiredService<IDbContextFactory<ApplicationDbContext>>().CreateDbContext());
}
*/

// Configure Settings
builder.Services.Configure<InstitutionSettings>(
    builder.Configuration.GetSection("InstitutionSettings"));
builder.Services.Configure<SeoSettings>(
    builder.Configuration.GetSection("SeoSettings"));

builder.Services.AddHttpContextAccessor();

// Register Services
builder.Services.AddScoped<CollegeWebSite.Services.Interfaces.IPageService, CollegeWebSite.Services.Implementation.PageService>();
builder.Services.AddScoped<CollegeWebSite.Services.Interfaces.IInquiryService, CollegeWebSite.Services.Implementation.InquiryService>();
builder.Services.AddScoped<CollegeWebSite.Services.Interfaces.IGalleryService, CollegeWebSite.Services.Implementation.GalleryService>();
builder.Services.AddScoped<CollegeWebSite.Services.Interfaces.IFacultyService, CollegeWebSite.Services.Implementation.FacultyService>();
builder.Services.AddScoped<CollegeWebSite.Services.Interfaces.IEventService, CollegeWebSite.Services.Implementation.EventService>();
builder.Services.AddScoped<CollegeWebSite.Services.Interfaces.IPlacementService, CollegeWebSite.Services.Implementation.PlacementService>();
builder.Services.AddScoped<CollegeWebSite.Services.Interfaces.IResultService, CollegeWebSite.Services.Implementation.ResultService>();
builder.Services.AddScoped<CollegeWebSite.Services.Interfaces.INoticeService, CollegeWebSite.Services.Implementation.NoticeService>();
builder.Services.AddScoped<CollegeWebSite.Services.Interfaces.ISyllabusService, CollegeWebSite.Services.Implementation.SyllabusService>();
builder.Services.AddScoped<CollegeWebSite.Services.Interfaces.IMediaService, CollegeWebSite.Services.Implementation.MediaService>();
builder.Services.AddScoped<CollegeWebSite.Services.Interfaces.ISeoService, CollegeWebSite.Services.Implementation.SeoService>();
builder.Services.AddScoped<CollegeWebSite.Services.Interfaces.IDocumentService, CollegeWebSite.Services.Implementation.DocumentService>();
builder.Services.AddScoped<CollegeWebSite.Services.Implementation.ThemeService>();
builder.Services.AddScoped<CollegeWebSite.Services.Interfaces.IHeroService, CollegeWebSite.Services.Implementation.HeroService>();
builder.Services.AddScoped<CollegeWebSite.Services.Interfaces.IPopupService, CollegeWebSite.Services.Implementation.PopupService>();
builder.Services.AddSingleton<CollegeWebSite.Services.Interfaces.IDialogService, CollegeWebSite.Services.Implementation.DialogService>();
builder.Services.AddScoped<IDbInitializer, DbInitializer>();

// builder.Logging.AddConsole();
// builder.Logging.AddDebug();

var app = builder.Build();

// Initialize Database
Log.Information("Step: Commencing database synchronization...");
using (var scope = app.Services.CreateScope())
{
    try 
    {
        var initializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        await initializer.InitializeAsync();
        Log.Information("Step: Database synchronization completed successfully.");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Critical: Database initialization failed during startup.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        // Add caching headers for static assets
        ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=2592000");
    },
    ContentTypeProvider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider
    {
        Mappings = { [".webp"] = "image/webp" }
    }
});

app.UseSession();
app.UseAntiforgery();

app.MapSeoFeedsAndDiscovery();

// Document Download Endpoint
app.MapGet("/api/documents/{id}/download", async (long id, 
    CollegeWebSite.Services.Interfaces.IDocumentService documentService,
    IWebHostEnvironment env) =>
{
    var document = await documentService.GetDocumentByIdAsync(id);
    if (document == null || !document.IsPublic)
    {
        return Results.NotFound();
    }

    var filePath = Path.Combine(env.WebRootPath, document.Media.FilePath);
    if (!System.IO.File.Exists(filePath))
    {
        return Results.NotFound();
    }

    var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
    return Results.File(fileBytes, document.Media.MimeType, document.Media.FileName);
});

// Static assets (optimized for performance)
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapControllers();

app.Run();
