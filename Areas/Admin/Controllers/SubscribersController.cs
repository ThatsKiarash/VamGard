using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VamYab.Data;

namespace VamYab.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class SubscribersController : Controller
{
    private readonly ApplicationDbContext _db;

    public SubscribersController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var subscribers = await _db.NewsletterSubscribers
            .OrderByDescending(s => s.SubscribedAt)
            .ToListAsync();
        return View(subscribers);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var sub = await _db.NewsletterSubscribers.FindAsync(id);
        if (sub == null) return NotFound();

        _db.NewsletterSubscribers.Remove(sub);
        await _db.SaveChangesAsync();
        TempData["Success"] = "اشتراک حذف شد";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExportCsv()
    {
        var subscribers = await _db.NewsletterSubscribers
            .Where(s => s.IsActive)
            .OrderByDescending(s => s.SubscribedAt)
            .ToListAsync();

        var csv = "Email,Name,SubscribedAt\n" +
                  string.Join("\n", subscribers.Select(s => $"{s.Email},{s.Name},{s.SubscribedAt:yyyy-MM-dd HH:mm}"));

        return File(System.Text.Encoding.UTF8.GetBytes(csv), "text/csv", "subscribers.csv");
    }
}
