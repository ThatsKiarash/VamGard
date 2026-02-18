using VamYab.Data;
using VamYab.Models;

namespace VamYab.Middleware;

public class AnalyticsMiddleware
{
    private readonly RequestDelegate _next;
    private static readonly HashSet<string> IgnoredExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".css", ".js", ".png", ".jpg", ".jpeg", ".gif", ".svg", ".ico", ".woff", ".woff2", ".ttf", ".map"
    };

    public AnalyticsMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ApplicationDbContext db)
    {
        await _next(context);

        try
        {
            var path = context.Request.Path.Value ?? "/";

            if (ShouldTrack(path, context))
            {
                var ip = context.Connection.RemoteIpAddress?.ToString();
                var ua = context.Request.Headers.UserAgent.ToString();

                if (!string.IsNullOrEmpty(ua) && !IsBot(ua))
                {
                    db.PageVisits.Add(new PageVisit
                    {
                        Path = path.Length > 500 ? path[..500] : path,
                        IpAddress = ip?.Length > 50 ? ip[..50] : ip,
                        UserAgent = ua.Length > 500 ? ua[..500] : ua,
                        VisitedAt = DateTime.UtcNow
                    });
                    await db.SaveChangesAsync();
                }
            }
        }
        catch
        {
            // Don't let analytics errors break the app
        }
    }

    private static bool ShouldTrack(string path, HttpContext context)
    {
        if (context.Response.StatusCode >= 400) return false;
        if (!string.Equals(context.Request.Method, "GET", StringComparison.OrdinalIgnoreCase)) return false;
        if (path.StartsWith("/Admin", StringComparison.OrdinalIgnoreCase)) return false;
        if (path.StartsWith("/api", StringComparison.OrdinalIgnoreCase)) return false;
        if (path == "/sitemap.xml") return false;

        var ext = System.IO.Path.GetExtension(path);
        if (!string.IsNullOrEmpty(ext) && IgnoredExtensions.Contains(ext)) return false;

        return true;
    }

    private static bool IsBot(string userAgent)
    {
        var ua = userAgent.ToLowerInvariant();
        return ua.Contains("bot") || ua.Contains("crawler") || ua.Contains("spider") ||
               ua.Contains("slurp") || ua.Contains("curl") || ua.Contains("wget");
    }
}
