using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VamYab.Data;
using VamYab.Models;

namespace VamYab.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class LoanTypesController : Controller
{
    private readonly ApplicationDbContext _db;

    public LoanTypesController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var types = await _db.LoanTypes.OrderBy(lt => lt.DisplayOrder).ToListAsync();
        return View(types);
    }

    public IActionResult Create()
    {
        return View(new LoanType());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LoanType loanType)
    {
        if (string.IsNullOrWhiteSpace(loanType.Slug))
            loanType.Slug = loanType.Name.Replace(" ", "-").Replace("‌", "-").ToLowerInvariant();

        if (await _db.LoanTypes.AnyAsync(lt => lt.Slug == loanType.Slug))
            ModelState.AddModelError("Slug", "این اسلاگ قبلاً استفاده شده است");

        if (!ModelState.IsValid)
            return View(loanType);

        _db.LoanTypes.Add(loanType);
        await _db.SaveChangesAsync();
        TempData["Success"] = "نوع وام با موفقیت اضافه شد";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var loanType = await _db.LoanTypes.FindAsync(id);
        if (loanType == null) return NotFound();
        return View(loanType);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, LoanType loanType)
    {
        if (id != loanType.Id) return NotFound();

        if (string.IsNullOrWhiteSpace(loanType.Slug))
            loanType.Slug = loanType.Name.Replace(" ", "-").Replace("‌", "-").ToLowerInvariant();

        if (await _db.LoanTypes.AnyAsync(lt => lt.Slug == loanType.Slug && lt.Id != id))
            ModelState.AddModelError("Slug", "این اسلاگ قبلاً استفاده شده است");

        if (!ModelState.IsValid)
            return View(loanType);

        var existing = await _db.LoanTypes.FindAsync(id);
        if (existing == null) return NotFound();

        existing.Name = loanType.Name;
        existing.Slug = loanType.Slug;
        existing.Description = loanType.Description;
        existing.IconClass = loanType.IconClass;
        existing.IsActive = loanType.IsActive;
        existing.DisplayOrder = loanType.DisplayOrder;

        await _db.SaveChangesAsync();
        TempData["Success"] = "نوع وام با موفقیت ویرایش شد";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var loanType = await _db.LoanTypes.FindAsync(id);
        if (loanType == null) return NotFound();

        _db.LoanTypes.Remove(loanType);
        await _db.SaveChangesAsync();
        TempData["Success"] = "نوع وام با موفقیت حذف شد";
        return RedirectToAction(nameof(Index));
    }
}
