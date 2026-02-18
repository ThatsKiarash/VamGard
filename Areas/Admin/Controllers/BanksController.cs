using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VamYab.Data;
using VamYab.Models;

namespace VamYab.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class BanksController : Controller
{
    private readonly ApplicationDbContext _db;

    public BanksController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var banks = await _db.Banks.OrderBy(b => b.DisplayOrder).ToListAsync();
        return View(banks);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateDropdowns();
        return View(new Bank());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Bank bank)
    {
        if (string.IsNullOrWhiteSpace(bank.Slug))
            bank.Slug = GenerateSlug(bank.Name);

        if (await _db.Banks.AnyAsync(b => b.Slug == bank.Slug))
            ModelState.AddModelError("Slug", "این اسلاگ قبلاً استفاده شده است");

        if (!ModelState.IsValid)
        {
            await PopulateDropdowns();
            return View(bank);
        }

        bank.CreatedAt = DateTime.UtcNow;
        _db.Banks.Add(bank);
        await _db.SaveChangesAsync();
        TempData["Success"] = "بانک با موفقیت اضافه شد";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var bank = await _db.Banks.FindAsync(id);
        if (bank == null) return NotFound();
        await PopulateDropdowns();
        return View(bank);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Bank bank)
    {
        if (id != bank.Id) return NotFound();

        if (string.IsNullOrWhiteSpace(bank.Slug))
            bank.Slug = GenerateSlug(bank.Name);

        if (await _db.Banks.AnyAsync(b => b.Slug == bank.Slug && b.Id != id))
            ModelState.AddModelError("Slug", "این اسلاگ قبلاً استفاده شده است");

        if (!ModelState.IsValid)
        {
            await PopulateDropdowns();
            return View(bank);
        }

        var existing = await _db.Banks.FindAsync(id);
        if (existing == null) return NotFound();

        existing.Name = bank.Name;
        existing.Slug = bank.Slug;
        existing.LogoUrl = bank.LogoUrl;
        existing.Website = bank.Website;
        existing.Description = bank.Description;
        existing.History = bank.History;
        existing.FoundedYear = bank.FoundedYear;
        existing.BranchCount = bank.BranchCount;
        existing.HeadquartersCity = bank.HeadquartersCity;
        existing.Latitude = bank.Latitude;
        existing.Longitude = bank.Longitude;
        existing.BankType = bank.BankType;
        existing.ParentBankId = bank.ParentBankId;
        existing.OwnershipType = bank.OwnershipType;
        existing.IsActive = bank.IsActive;
        existing.DisplayOrder = bank.DisplayOrder;
        existing.Address = bank.Address;
        existing.PhoneNumber = bank.PhoneNumber;
        existing.Email = bank.Email;
        existing.BranchesJson = bank.BranchesJson;
        existing.MetaTitle = bank.MetaTitle;
        existing.MetaDescription = bank.MetaDescription;

        await _db.SaveChangesAsync();
        TempData["Success"] = "بانک با موفقیت ویرایش شد";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> ExportJson(int id)
    {
        var bank = await _db.Banks.FindAsync(id);
        if (bank == null) return NotFound();

        var export = new Dictionary<string, object?>
        {
            ["_راهنما"] = "اطلاعات بانک از سایت وام‌گرد. فیلدهایی که میخواهید تغییر دهید را ارسال کنید.",
            ["_فیلدها"] = new Dictionary<string, string>
            {
                ["Name"] = "نام بانک (فارسی)",
                ["Slug"] = "نامک URL (انگلیسی با خط تیره)",
                ["LogoUrl"] = "آدرس لوگو",
                ["Website"] = "آدرس وب‌سایت رسمی",
                ["Description"] = "توضیحات بانک",
                ["History"] = "تاریخچه بانک",
                ["FoundedYear"] = "سال تأسیس (عدد)",
                ["BranchCount"] = "تعداد شعب (عدد)",
                ["HeadquartersCity"] = "شهر مرکزی",
                ["Address"] = "آدرس دفتر مرکزی",
                ["PhoneNumber"] = "شماره تلفن",
                ["Email"] = "ایمیل",
                ["Latitude"] = "عرض جغرافیایی",
                ["Longitude"] = "طول جغرافیایی",
                ["BankType"] = "نوع: bank یا neobank",
                ["OwnershipType"] = "نوع مالکیت: دولتی / نیمه‌دولتی / خصوصی / سهامی عام / ترابانک",
                ["BranchesJson"] = "لیست شعب به صورت JSON آرایه: [{\"name\":\"شعبه مرکزی\",\"address\":\"تهران ...\",\"lat\":35.69,\"lng\":51.39,\"phone\":\"021...\"}]",
                ["MetaTitle"] = "عنوان سئو",
                ["MetaDescription"] = "توضیحات سئو"
            },
            ["Name"] = bank.Name,
            ["Slug"] = bank.Slug,
            ["LogoUrl"] = bank.LogoUrl,
            ["Website"] = bank.Website,
            ["Description"] = bank.Description,
            ["History"] = bank.History,
            ["FoundedYear"] = bank.FoundedYear,
            ["BranchCount"] = bank.BranchCount,
            ["HeadquartersCity"] = bank.HeadquartersCity,
            ["Address"] = bank.Address,
            ["PhoneNumber"] = bank.PhoneNumber,
            ["Email"] = bank.Email,
            ["Latitude"] = bank.Latitude,
            ["Longitude"] = bank.Longitude,
            ["BankType"] = bank.BankType,
            ["OwnershipType"] = bank.OwnershipType,
            ["BranchesJson"] = bank.BranchesJson,
            ["IsActive"] = bank.IsActive,
            ["DisplayOrder"] = bank.DisplayOrder,
            ["MetaTitle"] = bank.MetaTitle,
            ["MetaDescription"] = bank.MetaDescription
        };

        var json = System.Text.Json.JsonSerializer.Serialize(export, new System.Text.Json.JsonSerializerOptions { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
        return Content(json, "application/json");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ImportJson(int id, string jsonData)
    {
        var bank = await _db.Banks.FindAsync(id);
        if (bank == null) return NotFound();
        try
        {
            var data = System.Text.Json.JsonDocument.Parse(jsonData);
            var root = data.RootElement;
            if (root.TryGetProperty("Name", out var v)) bank.Name = v.GetString() ?? bank.Name;
            if (root.TryGetProperty("Slug", out v)) bank.Slug = v.GetString() ?? bank.Slug;
            if (root.TryGetProperty("LogoUrl", out v)) bank.LogoUrl = v.GetString();
            if (root.TryGetProperty("Website", out v)) bank.Website = v.GetString();
            if (root.TryGetProperty("Description", out v)) bank.Description = v.GetString();
            if (root.TryGetProperty("History", out v)) bank.History = v.GetString();
            if (root.TryGetProperty("FoundedYear", out v)) bank.FoundedYear = v.ValueKind == System.Text.Json.JsonValueKind.Null ? null : v.GetInt32();
            if (root.TryGetProperty("BranchCount", out v)) bank.BranchCount = v.ValueKind == System.Text.Json.JsonValueKind.Null ? null : v.GetInt32();
            if (root.TryGetProperty("HeadquartersCity", out v)) bank.HeadquartersCity = v.GetString();
            if (root.TryGetProperty("Address", out v)) bank.Address = v.GetString();
            if (root.TryGetProperty("PhoneNumber", out v)) bank.PhoneNumber = v.GetString();
            if (root.TryGetProperty("Email", out v)) bank.Email = v.GetString();
            if (root.TryGetProperty("Latitude", out v)) bank.Latitude = v.ValueKind == System.Text.Json.JsonValueKind.Null ? null : v.GetDouble();
            if (root.TryGetProperty("Longitude", out v)) bank.Longitude = v.ValueKind == System.Text.Json.JsonValueKind.Null ? null : v.GetDouble();
            if (root.TryGetProperty("BankType", out v)) bank.BankType = v.GetString() ?? bank.BankType;
            if (root.TryGetProperty("OwnershipType", out v)) bank.OwnershipType = v.GetString();
            if (root.TryGetProperty("BranchesJson", out v)) bank.BranchesJson = v.ValueKind == System.Text.Json.JsonValueKind.String ? v.GetString() : v.GetRawText();
            if (root.TryGetProperty("IsActive", out v)) bank.IsActive = v.GetBoolean();
            if (root.TryGetProperty("DisplayOrder", out v)) bank.DisplayOrder = v.GetInt32();
            if (root.TryGetProperty("MetaTitle", out v)) bank.MetaTitle = v.GetString();
            if (root.TryGetProperty("MetaDescription", out v)) bank.MetaDescription = v.GetString();
            await _db.SaveChangesAsync();
            TempData["Success"] = "بانک با موفقیت از JSON بروزرسانی شد";
        }
        catch (Exception ex)
        {
            TempData["Error"] = "خطا در پردازش JSON: " + ex.Message;
        }
        return RedirectToAction(nameof(Edit), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var bank = await _db.Banks.FindAsync(id);
        if (bank == null) return NotFound();

        _db.Banks.Remove(bank);
        await _db.SaveChangesAsync();
        TempData["Success"] = "بانک با موفقیت حذف شد";
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdowns()
    {
        ViewBag.ParentBanks = await _db.Banks
            .Where(b => b.BankType == "bank")
            .OrderBy(b => b.DisplayOrder)
            .Select(b => new { b.Id, b.Name })
            .ToListAsync();
    }

    private static string GenerateSlug(string name)
    {
        return name.Replace(" ", "-").Replace("‌", "-").ToLowerInvariant();
    }
}
