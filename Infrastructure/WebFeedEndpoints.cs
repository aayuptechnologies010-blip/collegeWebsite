using System.Text;
using CollegeWebSite.Domain.Settings;
using CollegeWebSite.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace CollegeWebSite.Infrastructure;

public static class WebFeedEndpoints
{
    public static void MapSeoFeedsAndDiscovery(this WebApplication app)
    {
        app.MapGet("/sitemap.xml", async (
            HttpContext context,
            IPageService pageService,
            IEventService eventService) =>
        {
            var baseUrl = $"{context.Request.Scheme}://{context.Request.Host}";
            var pages = await pageService.GetAllPagesAsync();
            var publishedPages = pages.Where(p => p.IsPublished).ToList();

            var sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");

            void AddUrl(string path, DateTime lastMod, string changefreq, string priority)
            {
                sb.AppendLine("  <url>");
                sb.AppendLine($"    <loc>{baseUrl}{path}</loc>");
                sb.AppendLine($"    <lastmod>{lastMod:yyyy-MM-dd}</lastmod>");
                sb.AppendLine($"    <changefreq>{changefreq}</changefreq>");
                sb.AppendLine($"    <priority>{priority}</priority>");
                sb.AppendLine("  </url>");
            }

            var today = DateTime.UtcNow.Date;
            AddUrl("/", today, "daily", "1.0");
            AddUrl("/p/contact", today, "monthly", "0.9");
            AddUrl("/notices", today, "weekly", "0.75");
            AddUrl("/faculty", today, "weekly", "0.8");
            AddUrl("/gallery", today, "weekly", "0.7");
            AddUrl("/events", today, "weekly", "0.75");
            AddUrl("/apply", today, "monthly", "0.9");
            AddUrl("/placements", today, "monthly", "0.7");
            AddUrl("/curriculum", today, "weekly", "0.8");
            AddUrl("/results", today, "monthly", "0.7");

            foreach (var p in publishedPages)
            {
                var lastMod = (p.LastModifiedOn ?? p.CreatedOn).ToUniversalTime().Date;
                var path = "/p/" + p.Slug.TrimStart('/');
                AddUrl(path, lastMod, "weekly", "0.6");
            }

            sb.AppendLine("</urlset>");

            context.Response.Headers.Append("Cache-Control", "public, max-age=3600");
            return Results.Content(sb.ToString(), "application/xml; charset=utf-8");
        });

        app.MapGet("/robots.txt", (HttpContext context, IOptions<SeoSettings> seoOptions) =>
        {
            var baseUrl = $"{context.Request.Scheme}://{context.Request.Host}";
            var host = context.Request.Host.Host;
            var site = seoOptions.Value.SiteName;

            var robots = $@"# {site}
User-agent: *
Allow: /
Disallow: /admin/
Disallow: /api/
Disallow: /_framework/
Disallow: /_blazor/

User-agent: Googlebot
Allow: /

User-agent: Bingbot
Allow: /

# Discovery
Sitemap: {baseUrl}/sitemap.xml

# RSS / Atom-style feeds (for readers & aggregators)
# Main: {baseUrl}/rss.xml
# Events: {baseUrl}/feed/events.xml
# Notices: {baseUrl}/feed/notices.xml
{(host.Contains("localhost", StringComparison.OrdinalIgnoreCase) ? "" : $"Host: {host}\n")}
";
            context.Response.ContentType = "text/plain; charset=utf-8";
            context.Response.Headers.Append("Cache-Control", "public, max-age=86400");
            return Results.Content(robots, "text/plain; charset=utf-8");
        });

        app.MapGet("/rss.xml", async (
            HttpContext context,
            IPageService pageService,
            IEventService eventService,
            INoticeService noticeService,
            IOptions<SeoSettings> seoOptions) =>
        {
            var baseUrl = $"{context.Request.Scheme}://{context.Request.Host}";
            var seo = seoOptions.Value;
            var items = await BuildCombinedFeedItemsAsync(baseUrl, pageService, eventService, noticeService);
            var xml = BuildRssDocument(
                baseUrl,
                $"{baseUrl}/rss.xml",
                $"{seo.SiteName} — News & updates",
                SyndicationXml.ToPlainSummary(seo.DefaultDescription, 300),
                items.Take(60));
            context.Response.ContentType = "application/rss+xml; charset=utf-8";
            context.Response.Headers.Append("Cache-Control", "public, max-age=1800");
            return Results.Content(xml, "application/rss+xml; charset=utf-8");
        });

        app.MapGet("/feed/events.xml", async (
            HttpContext context,
            IEventService eventService,
            IOptions<SeoSettings> seoOptions) =>
        {
            var baseUrl = $"{context.Request.Scheme}://{context.Request.Host}";
            var seo = seoOptions.Value;
            var events = await eventService.GetAllEventsAsync(onlyPublished: true);
            var items = events
                .OrderByDescending(e => e.EventDate)
                .Take(50)
                .Select(e => new RssItem(
                    $"{baseUrl}/events",
                    $"event-{e.Id}",
                    e.Title,
                    SyndicationXml.ToPlainSummary(e.Description, 400),
                    e.PublishedOn ?? e.EventDate));
            var xml = BuildRssDocument(
                baseUrl,
                $"{baseUrl}/feed/events.xml",
                $"{seo.SiteName} — Campus events",
                "Workshops, seminars, and campus events.",
                items);
            context.Response.ContentType = "application/rss+xml; charset=utf-8";
            context.Response.Headers.Append("Cache-Control", "public, max-age=1800");
            return Results.Content(xml, "application/rss+xml; charset=utf-8");
        });

        app.MapGet("/feed/notices.xml", async (
            HttpContext context,
            INoticeService noticeService,
            IOptions<SeoSettings> seoOptions) =>
        {
            var baseUrl = $"{context.Request.Scheme}://{context.Request.Host}";
            var seo = seoOptions.Value;
            var notices = await noticeService.GetActiveNoticesAsync();
            var items = notices
                .OrderByDescending(n => n.CreatedOn)
                .Take(50)
                .Select(n => new RssItem(
                    SafeNoticeLink(baseUrl, n.LinkUrl),
                    $"notice-{n.Id}",
                    n.Title,
                    SyndicationXml.ToPlainSummary(n.Content, 400),
                    n.CreatedOn));
            var xml = BuildRssDocument(
                baseUrl,
                $"{baseUrl}/feed/notices.xml",
                $"{seo.SiteName} — Notices",
                "Official notices and announcements.",
                items);
            context.Response.ContentType = "application/rss+xml; charset=utf-8";
            context.Response.Headers.Append("Cache-Control", "public, max-age=900");
            return Results.Content(xml, "application/rss+xml; charset=utf-8");
        });
    }

    private sealed record RssItem(string Link, string GuidValue, string Title, string Description, DateTime PubDateUtc);

    private static string SafeNoticeLink(string baseUrl, string? linkUrl)
    {
        if (string.IsNullOrWhiteSpace(linkUrl)) return $"{baseUrl.TrimEnd('/')}/notices";
        var t = linkUrl.Trim();
        if (t.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || t.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            return t;
        if (t.StartsWith('/')) return $"{baseUrl.TrimEnd('/')}{t}";
        return $"{baseUrl.TrimEnd('/')}/notices";
    }

    private static async Task<List<RssItem>> BuildCombinedFeedItemsAsync(
        string baseUrl,
        IPageService pageService,
        IEventService eventService,
        INoticeService noticeService)
    {
        var list = new List<RssItem>();

        foreach (var e in (await eventService.GetAllEventsAsync(onlyPublished: true)).OrderByDescending(x => x.EventDate).Take(25))
        {
            list.Add(new RssItem(
                $"{baseUrl}/events",
                $"event-{e.Id}",
                e.Title,
                SyndicationXml.ToPlainSummary(e.Description, 350),
                e.PublishedOn ?? e.EventDate));
        }

        foreach (var n in (await noticeService.GetActiveNoticesAsync()).OrderByDescending(x => x.CreatedOn).Take(25))
        {
            list.Add(new RssItem(
                SafeNoticeLink(baseUrl, n.LinkUrl),
                $"notice-{n.Id}",
                n.Title,
                SyndicationXml.ToPlainSummary(n.Content, 350),
                n.CreatedOn));
        }

        foreach (var p in (await pageService.GetAllPagesAsync()).Where(x => x.IsPublished).OrderByDescending(x => x.LastModifiedOn ?? x.CreatedOn).Take(15))
        {
            var desc = p.Seo?.MetaDescription;
            if (string.IsNullOrWhiteSpace(desc))
                desc = p.Title;
            list.Add(new RssItem(
                $"{baseUrl}/p/{p.Slug.TrimStart('/')}",
                $"page-{p.Id}",
                p.Title,
                SyndicationXml.ToPlainSummary(desc, 350),
                (p.LastModifiedOn ?? p.CreatedOn).ToUniversalTime()));
        }

        return list.OrderByDescending(i => i.PubDateUtc).ToList();
    }

    private static string BuildRssDocument(string baseUrl, string selfLink, string title, string description, IEnumerable<RssItem> items)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        sb.AppendLine("<rss version=\"2.0\" xmlns:atom=\"http://www.w3.org/2005/Atom\">");
        sb.AppendLine("  <channel>");
        sb.AppendLine($"    <title>{SyndicationXml.Escape(title)}</title>");
        sb.AppendLine($"    <link>{SyndicationXml.Escape(baseUrl + "/")}</link>");
        sb.AppendLine($"    <description>{SyndicationXml.Escape(description)}</description>");
        sb.AppendLine($"    <language>en-in</language>");
        sb.AppendLine($"    <lastBuildDate>{SyndicationXml.Rfc822Date(DateTime.UtcNow)}</lastBuildDate>");
        sb.AppendLine($"    <atom:link href=\"{SyndicationXml.Escape(selfLink)}\" rel=\"self\" type=\"application/rss+xml\" />");
        sb.AppendLine($"    <image>");
        sb.AppendLine($"      <url>{SyndicationXml.Escape($"{baseUrl.TrimEnd('/')}/favicon.png")}</url>");
        sb.AppendLine($"      <title>{SyndicationXml.Escape(title)}</title>");
        sb.AppendLine($"      <link>{SyndicationXml.Escape(baseUrl + "/")}</link>");
        sb.AppendLine($"    </image>");

        foreach (var item in items)
        {
            sb.AppendLine("    <item>");
            sb.AppendLine($"      <title>{SyndicationXml.Escape(item.Title)}</title>");
            sb.AppendLine($"      <link>{SyndicationXml.Escape(item.Link)}</link>");
            sb.AppendLine($"      <guid isPermaLink=\"false\">{SyndicationXml.Escape(item.GuidValue)}</guid>");
            sb.AppendLine($"      <pubDate>{SyndicationXml.Rfc822Date(item.PubDateUtc)}</pubDate>");
            sb.AppendLine($"      <description>{SyndicationXml.Escape(item.Description)}</description>");
            sb.AppendLine("    </item>");
        }

        sb.AppendLine("  </channel>");
        sb.AppendLine("</rss>");
        return sb.ToString();
    }
}
