using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VamYab.Data;
using VamYab.Models;

namespace VamYab.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _db;

    public DashboardController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.TotalBanks = await _db.Banks.CountAsync();
        ViewBag.TotalLoanTypes = await _db.LoanTypes.CountAsync();
        ViewBag.TotalLoans = await _db.Loans.CountAsync();
        ViewBag.ActiveLoans = await _db.Loans.CountAsync(l => l.IsActive);
        ViewBag.TotalSubscribers = await _db.NewsletterSubscribers.CountAsync();
        ViewBag.TotalBlogPosts = await _db.BlogPosts.CountAsync(p => p.IsPublished);

        ViewBag.RecentLoans = await _db.Loans
            .Include(l => l.Bank)
            .Include(l => l.LoanType)
            .OrderByDescending(l => l.UpdatedAt)
            .Take(5)
            .ToListAsync();

        ViewBag.RecentVisitors = await _db.PageVisits
            .OrderByDescending(v => v.VisitedAt)
            .Take(100)
            .ToListAsync();

        return View();
    }

    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
    {
        if (newPassword != confirmPassword)
        {
            TempData["Error"] = "رمز عبور جدید و تکرار آن مطابقت ندارند";
            return View();
        }
        if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
        {
            TempData["Error"] = "رمز عبور جدید باید حداقل ۶ کاراکتر باشد";
            return View();
        }
        var username = User.Identity?.Name;
        var admin = await _db.AdminUsers.FirstOrDefaultAsync(a => a.Username == username);
        if (admin == null) return NotFound();

        var hasher = HttpContext.RequestServices.GetRequiredService<Microsoft.AspNetCore.Identity.IPasswordHasher<AdminUser>>();
        var result = hasher.VerifyHashedPassword(admin, admin.PasswordHash, currentPassword);
        if (result == Microsoft.AspNetCore.Identity.PasswordVerificationResult.Failed)
        {
            TempData["Error"] = "رمز عبور فعلی اشتباه است";
            return View();
        }
        admin.PasswordHash = hasher.HashPassword(admin, newPassword);
        await _db.SaveChangesAsync();
        TempData["Success"] = "رمز عبور با موفقیت تغییر کرد";
        return RedirectToAction(nameof(Index));
    }
}
