using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;

namespace CollegeWebSite.Infrastructure;

/// <summary>RSS 2.0 and sitemap-safe XML helpers.</summary>
public static class SyndicationXml
{
    public static string Escape(string? text)
    {
        if (string.IsNullOrEmpty(text)) return "";
        return WebUtility.HtmlEncode(text);
    }

    /// <summary>Strip tags for RSS description; collapse whitespace.</summary>
    public static string ToPlainSummary(string? htmlOrText, int maxLength = 500)
    {
        if (string.IsNullOrWhiteSpace(htmlOrText)) return "";
        var s = Regex.Replace(htmlOrText, "<[^>]+>", " ");
        s = WebUtility.HtmlDecode(s);
        s = Regex.Replace(s, @"\s+", " ").Trim();
        if (s.Length <= maxLength) return s;
        return s[..(maxLength - 1)].TrimEnd() + "…";
    }

    public static string Rfc822Date(DateTime utc)
    {
        utc = utc.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(utc, DateTimeKind.Utc) : utc.ToUniversalTime();
        return utc.ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'", CultureInfo.InvariantCulture);
    }
}
