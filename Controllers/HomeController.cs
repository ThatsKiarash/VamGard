using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VamYab.Data;
using VamYab.Models;
using VamYab.Services;

namespace VamYab.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly EmailService _email;

    public HomeController(ApplicationDbContext db, EmailService email)
    {
        _db = db;
        _email = email;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.FeaturedLoans = await _db.Loans
            .Include(l => l.Bank)
            .Include(l => l.LoanType)
            .Where(l => l.IsActive && l.IsFeatured)
            .OrderByDescending(l => l.UpdatedAt)
            .Take(6)
            .ToListAsync();

        ViewBag.HotLoans = await _db.Loans
            .Include(l => l.Bank)
            .Include(l => l.LoanType)
            .Where(l => l.IsActive)
            .OrderByDescending(l => l.ViewCount)
            .ThenByDescending(l => l.UpdatedAt)
            .Take(6)
            .ToListAsync();

        ViewBag.LatestLoans = await _db.Loans
            .Include(l => l.Bank)
            .Include(l => l.LoanType)
            .Where(l => l.IsActive)
            .OrderByDescending(l => l.CreatedAt)
            .Take(6)
            .ToListAsync();

        ViewBag.Banks = await _db.Banks
            .Include(b => b.Loans.Where(l => l.IsActive))
                .ThenInclude(l => l.LoanType)
            .Where(b => b.IsActive)
            .OrderBy(b => b.DisplayOrder)
            .ToListAsync();

        ViewBag.LoanTypes = await _db.LoanTypes
            .Where(lt => lt.IsActive)
            .OrderBy(lt => lt.DisplayOrder)
            .ToListAsync();

        ViewBag.TotalLoans = await _db.Loans.CountAsync(l => l.IsActive);
        ViewBag.TotalBanks = await _db.Banks.CountAsync(b => b.IsActive);
        ViewBag.TotalLoanTypes = await _db.LoanTypes.CountAsync(lt => lt.IsActive);

        ViewBag.BubbleLoans = await _db.Loans
            .Include(l => l.Bank)
            .Include(l => l.LoanType)
            .Where(l => l.IsActive && l.Bank.LogoUrl != null)
            .OrderByDescending(l => l.ViewCount)
            .Take(12)
            .ToListAsync();

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Subscribe(string email, string? name)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            return Json(new { success = false, message = "لطفاً یک ایمیل معتبر وارد کنید." });

        email = email.Trim().ToLowerInvariant();

        if (await _db.NewsletterSubscribers.AnyAsync(s => s.Email == email))
            return Json(new { success = true, message = "این ایمیل قبلاً در خبرنامه ثبت شده است." });

        var subscriber = new NewsletterSubscriber
        {
            Email = email,
            Name = name?.Trim(),
            IsActive = true,
            SubscribedAt = DateTime.UtcNow,
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
        };

        _db.NewsletterSubscribers.Add(subscriber);
        await _db.SaveChangesAsync();

        _ = _email.SendWelcomeEmailAsync(email);

        return Json(new { success = true, message = "با موفقیت در خبرنامه وام‌گرد عضو شدید!" });
    }

    [Route("sitemap.xml")]
    public async Task<IActionResult> Sitemap()
    {
        var loans = await _db.Loans.Where(l => l.IsActive).ToListAsync();
        var banks = await _db.Banks.Where(b => b.IsActive).ToListAsync();
        var loanTypes = await _db.LoanTypes.Where(lt => lt.IsActive).ToListAsync();
        var blogPosts = await _db.BlogPosts.Where(p => p.IsPublished).ToListAsync();

        var sb = new System.Text.StringBuilder();
        sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        sb.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");

        sb.AppendLine("<url><loc>https://vamgard.org/</loc><changefreq>daily</changefreq><priority>1.0</priority></url>");
        sb.AppendLine("<url><loc>https://vamgard.org/Loans</loc><changefreq>daily</changefreq><priority>0.9</priority></url>");
        sb.AppendLine("<url><loc>https://vamgard.org/Banks</loc><changefreq>weekly</changefreq><priority>0.8</priority></url>");
        sb.AppendLine("<url><loc>https://vamgard.org/Blog</loc><changefreq>weekly</changefreq><priority>0.8</priority></url>");

        foreach (var loan in loans)
            sb.AppendLine($"<url><loc>https://vamgard.org/vam/{loan.Slug}</loc><lastmod>{loan.UpdatedAt:yyyy-MM-dd}</lastmod><changefreq>weekly</changefreq><priority>0.8</priority></url>");

        foreach (var bank in banks)
            sb.AppendLine($"<url><loc>https://vamgard.org/bank/{bank.Slug}</loc><changefreq>weekly</changefreq><priority>0.7</priority></url>");

        foreach (var lt in loanTypes)
            sb.AppendLine($"<url><loc>https://vamgard.org/type/{lt.Slug}</loc><changefreq>weekly</changefreq><priority>0.7</priority></url>");

        foreach (var post in blogPosts)
            sb.AppendLine($"<url><loc>https://vamgard.org/blog/{post.Slug}</loc><lastmod>{post.UpdatedAt:yyyy-MM-dd}</lastmod><changefreq>weekly</changefreq><priority>0.6</priority></url>");

        sb.AppendLine("</urlset>");

        return Content(sb.ToString(), "application/xml");
    }
}
