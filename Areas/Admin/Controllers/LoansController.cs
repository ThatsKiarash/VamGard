using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VamYab.Data;
using VamYab.Models;

namespace VamYab.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class LoansController : Controller
{
    private readonly ApplicationDbContext _db;

    public LoansController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var loans = await _db.Loans
            .Include(l => l.Bank)
            .Include(l => l.LoanType)
            .OrderByDescending(l => l.UpdatedAt)
            .ToListAsync();
        return View(loans);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateDropdowns();
        return View(new Loan());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Loan loan)
    {
        if (string.IsNullOrWhiteSpace(loan.Slug))
            loan.Slug = GenerateSlug(loan.Title);

        if (await _db.Loans.AnyAsync(l => l.Slug == loan.Slug))
            ModelState.AddModelError("Slug", "این اسلاگ قبلاً استفاده شده است");

        ModelState.Remove("Bank");
        ModelState.Remove("LoanType");

        if (!ModelState.IsValid)
        {
            await PopulateDropdowns();
            return View(loan);
        }

        loan.CreatedAt = DateTime.UtcNow;
        loan.UpdatedAt = DateTime.UtcNow;
        _db.Loans.Add(loan);
        await _db.SaveChangesAsync();
        TempData["Success"] = "وام با موفقیت اضافه شد";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var loan = await _db.Loans.FindAsync(id);
        if (loan == null) return NotFound();
        await PopulateDropdowns();
        return View(loan);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Loan loan)
    {
        if (id != loan.Id) return NotFound();

        if (string.IsNullOrWhiteSpace(loan.Slug))
            loan.Slug = GenerateSlug(loan.Title);

        if (await _db.Loans.AnyAsync(l => l.Slug == loan.Slug && l.Id != id))
            ModelState.AddModelError("Slug", "این اسلاگ قبلاً استفاده شده است");

        ModelState.Remove("Bank");
        ModelState.Remove("LoanType");

        if (!ModelState.IsValid)
        {
            await PopulateDropdowns();
            return View(loan);
        }

        var existing = await _db.Loans.FindAsync(id);
        if (existing == null) return NotFound();

        existing.Title = loan.Title;
        existing.Slug = loan.Slug;
        existing.ShortDescription = loan.ShortDescription;
        existing.FullDescription = loan.FullDescription;
        existing.InterestRate = loan.InterestRate;
        existing.MinAmount = loan.MinAmount;
        existing.MaxAmount = loan.MaxAmount;
        existing.RepaymentMonths = loan.RepaymentMonths;
        existing.Requirements = loan.Requirements;
        existing.IsActive = loan.IsActive;
        existing.IsFeatured = loan.IsFeatured;
        existing.BankId = loan.BankId;
        existing.LoanTypeId = loan.LoanTypeId;
        existing.ExternalUrl = loan.ExternalUrl;
        existing.MetaTitle = loan.MetaTitle;
        existing.MetaDescription = loan.MetaDescription;
        existing.MetaKeywords = loan.MetaKeywords;
        existing.AnalysisContent = loan.AnalysisContent;
        existing.HasCalculator = loan.HasCalculator;
        existing.CalcMinMonths = loan.CalcMinMonths;
        existing.CalcMaxMonths = loan.CalcMaxMonths;
        existing.CalcMonthStep = loan.CalcMonthStep;
        existing.CalcAdjustableRate = loan.CalcAdjustableRate;
        existing.CalcMinRate = loan.CalcMinRate;
        existing.CalcMaxRate = loan.CalcMaxRate;
        existing.RelatedArticleIds = loan.RelatedArticleIds;
        existing.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        TempData["Success"] = "وام با موفقیت ویرایش شد";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var loan = await _db.Loans.FindAsync(id);
        if (loan == null) return NotFound();

        _db.Loans.Remove(loan);
        await _db.SaveChangesAsync();
        TempData["Success"] = "وام با موفقیت حذف شد";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(int id)
    {
        var loan = await _db.Loans.FindAsync(id);
        if (loan == null) return NotFound();

        loan.IsActive = !loan.IsActive;
        loan.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        TempData["Success"] = loan.IsActive ? "وام فعال شد" : "وام غیرفعال شد";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> ExportJson(int id)
    {
        var loan = await _db.Loans.Include(l => l.Bank).Include(l => l.LoanType).FirstOrDefaultAsync(l => l.Id == id);
        if (loan == null) return NotFound();

        var bankName = loan.Bank?.Name ?? "";
        var loanTypeName = loan.LoanType?.Name ?? "";
        var banksList = await _db.Banks.Where(b => b.IsActive).Select(b => new { b.Id, b.Name }).ToListAsync();
        var typesList = await _db.LoanTypes.Where(lt => lt.IsActive).Select(lt => new { lt.Id, lt.Name }).ToListAsync();

        var export = new Dictionary<string, object?>
        {
            ["_راهنما"] = "این فایل JSON شامل اطلاعات یک وام بانکی از سایت وام‌گرد است. فیلدهای زیر قابل ویرایش هستند. برای وارد کردن، فقط فیلدهایی که میخواهید تغییر دهید را بفرستید.",
            ["_فیلدها"] = new Dictionary<string, string>
            {
                ["Title"] = "عنوان وام (فارسی) - مثال: وام ازدواج بانک ملی",
                ["Slug"] = "نامک URL (انگلیسی با خط تیره) - مثال: vam-ezdevaj-melli",
                ["ShortDescription"] = "توضیح کوتاه وام (۱-۲ جمله برای نمایش در لیست)",
                ["FullDescription"] = "توضیحات کامل وام (HTML مجاز) - شرایط، مزایا، نحوه دریافت",
                ["InterestRate"] = "نرخ سود سالانه به درصد - مثال: 23.0",
                ["MinAmount"] = "حداقل مبلغ وام به ریال - مثال: 500000000 (۵۰ میلیون تومان)",
                ["MaxAmount"] = "حداکثر مبلغ وام به ریال - مثال: 5000000000 (۵۰۰ میلیون تومان)",
                ["RepaymentMonths"] = "مدت بازپرداخت به ماه - مثال: 60",
                ["Requirements"] = "شرایط و مدارک لازم (HTML مجاز)",
                ["IsActive"] = "آیا وام فعال است؟ true/false",
                ["IsFeatured"] = "آیا وام ویژه است؟ true/false",
                ["BankId"] = "شناسه بانک (عدد) - لیست بانک‌ها در فیلد _بانک‌ها",
                ["LoanTypeId"] = "شناسه نوع وام (عدد) - لیست انواع در فیلد _انواع‌وام",
                ["ExternalUrl"] = "لینک صفحه وام در سایت رسمی بانک",
                ["MetaTitle"] = "عنوان سئو (تا ۶۰ کاراکتر)",
                ["MetaDescription"] = "توضیحات سئو (تا ۱۶۰ کاراکتر)",
                ["MetaKeywords"] = "کلمات کلیدی سئو (با کاما جدا شوند)",
                ["AnalysisContent"] = "تحلیل و بررسی تخصصی وام (HTML مجاز) - اختیاری",
                ["HasCalculator"] = "آیا ماشین‌حساب اقساط نمایش داده شود؟ true/false",
                ["CalcMinMonths"] = "حداقل مدت بازپرداخت در ماشین‌حساب (ماه) - مثال: 6",
                ["CalcMaxMonths"] = "حداکثر مدت بازپرداخت در ماشین‌حساب (ماه) - مثال: 60",
                ["CalcMonthStep"] = "گام تغییر مدت بازپرداخت (ماه) - مثال: 6",
                ["CalcAdjustableRate"] = "آیا نرخ سود در ماشین‌حساب قابل تغییر باشد؟ true/false",
                ["CalcMinRate"] = "حداقل نرخ سود قابل انتخاب - مثال: 4.0",
                ["CalcMaxRate"] = "حداکثر نرخ سود قابل انتخاب - مثال: 23.0",
                ["RelatedArticleIds"] = "شناسه مقالات مرتبط با کاما - مثال: 1,3,5"
            },
            ["_بانک‌ها"] = banksList.Select(b => $"{b.Id}: {b.Name}").ToList(),
            ["_انواع‌وام"] = typesList.Select(t => $"{t.Id}: {t.Name}").ToList(),
            ["_بانک_فعلی"] = $"{loan.BankId}: {bankName}",
            ["_نوع_فعلی"] = $"{loan.LoanTypeId}: {loanTypeName}",
            ["Title"] = loan.Title,
            ["Slug"] = loan.Slug,
            ["ShortDescription"] = loan.ShortDescription,
            ["FullDescription"] = loan.FullDescription,
            ["InterestRate"] = loan.InterestRate,
            ["MinAmount"] = loan.MinAmount,
            ["MaxAmount"] = loan.MaxAmount,
            ["RepaymentMonths"] = loan.RepaymentMonths,
            ["Requirements"] = loan.Requirements,
            ["IsActive"] = loan.IsActive,
            ["IsFeatured"] = loan.IsFeatured,
            ["BankId"] = loan.BankId,
            ["LoanTypeId"] = loan.LoanTypeId,
            ["ExternalUrl"] = loan.ExternalUrl,
            ["MetaTitle"] = loan.MetaTitle,
            ["MetaDescription"] = loan.MetaDescription,
            ["MetaKeywords"] = loan.MetaKeywords,
            ["AnalysisContent"] = loan.AnalysisContent,
            ["HasCalculator"] = loan.HasCalculator,
            ["CalcMinMonths"] = loan.CalcMinMonths,
            ["CalcMaxMonths"] = loan.CalcMaxMonths,
            ["CalcMonthStep"] = loan.CalcMonthStep,
            ["CalcAdjustableRate"] = loan.CalcAdjustableRate,
            ["CalcMinRate"] = loan.CalcMinRate,
            ["CalcMaxRate"] = loan.CalcMaxRate,
            ["RelatedArticleIds"] = loan.RelatedArticleIds
        };

        var json = System.Text.Json.JsonSerializer.Serialize(export, new System.Text.Json.JsonSerializerOptions { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
        return Content(json, "application/json");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ImportJson(int id, string jsonData)
    {
        var loan = await _db.Loans.FindAsync(id);
        if (loan == null) return NotFound();
        try
        {
            var data = System.Text.Json.JsonDocument.Parse(jsonData);
            var root = data.RootElement;
            if (root.TryGetProperty("Title", out var t)) loan.Title = t.GetString() ?? loan.Title;
            if (root.TryGetProperty("Slug", out var s)) loan.Slug = s.GetString() ?? loan.Slug;
            if (root.TryGetProperty("ShortDescription", out var sd)) loan.ShortDescription = sd.GetString();
            if (root.TryGetProperty("FullDescription", out var fd)) loan.FullDescription = fd.GetString();
            if (root.TryGetProperty("InterestRate", out var ir)) loan.InterestRate = ir.GetDecimal();
            if (root.TryGetProperty("MinAmount", out var mn)) loan.MinAmount = mn.GetInt64();
            if (root.TryGetProperty("MaxAmount", out var mx)) loan.MaxAmount = mx.GetInt64();
            if (root.TryGetProperty("RepaymentMonths", out var rm)) loan.RepaymentMonths = rm.GetInt32();
            if (root.TryGetProperty("Requirements", out var rq)) loan.Requirements = rq.GetString();
            if (root.TryGetProperty("IsActive", out var ia)) loan.IsActive = ia.GetBoolean();
            if (root.TryGetProperty("IsFeatured", out var ife)) loan.IsFeatured = ife.GetBoolean();
            if (root.TryGetProperty("BankId", out var bi)) loan.BankId = bi.GetInt32();
            if (root.TryGetProperty("LoanTypeId", out var lt)) loan.LoanTypeId = lt.GetInt32();
            if (root.TryGetProperty("ExternalUrl", out var eu)) loan.ExternalUrl = eu.GetString();
            if (root.TryGetProperty("MetaTitle", out var mt)) loan.MetaTitle = mt.GetString();
            if (root.TryGetProperty("MetaDescription", out var md)) loan.MetaDescription = md.GetString();
            if (root.TryGetProperty("MetaKeywords", out var mk)) loan.MetaKeywords = mk.GetString();
            if (root.TryGetProperty("AnalysisContent", out var ac)) loan.AnalysisContent = ac.GetString();
            if (root.TryGetProperty("HasCalculator", out var hc)) loan.HasCalculator = hc.GetBoolean();
            if (root.TryGetProperty("CalcMinMonths", out var cmn)) loan.CalcMinMonths = cmn.ValueKind == System.Text.Json.JsonValueKind.Null ? null : cmn.GetInt32();
            if (root.TryGetProperty("CalcMaxMonths", out var cmx)) loan.CalcMaxMonths = cmx.ValueKind == System.Text.Json.JsonValueKind.Null ? null : cmx.GetInt32();
            if (root.TryGetProperty("CalcMonthStep", out var cms)) loan.CalcMonthStep = cms.ValueKind == System.Text.Json.JsonValueKind.Null ? null : cms.GetInt32();
            if (root.TryGetProperty("CalcAdjustableRate", out var car)) loan.CalcAdjustableRate = car.GetBoolean();
            if (root.TryGetProperty("CalcMinRate", out var crn)) loan.CalcMinRate = crn.ValueKind == System.Text.Json.JsonValueKind.Null ? null : crn.GetDecimal();
            if (root.TryGetProperty("CalcMaxRate", out var crx)) loan.CalcMaxRate = crx.ValueKind == System.Text.Json.JsonValueKind.Null ? null : crx.GetDecimal();
            if (root.TryGetProperty("RelatedArticleIds", out var rai)) loan.RelatedArticleIds = rai.GetString();
            loan.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            TempData["Success"] = "وام با موفقیت از JSON بروزرسانی شد";
        }
        catch (Exception ex)
        {
            TempData["Error"] = "خطا در پردازش JSON: " + ex.Message;
        }
        return RedirectToAction(nameof(Edit), new { id });
    }

    private async Task PopulateDropdowns()
    {
        ViewBag.Banks = new SelectList(await _db.Banks.OrderBy(b => b.DisplayOrder).ToListAsync(), "Id", "Name");
        ViewBag.LoanTypes = new SelectList(await _db.LoanTypes.OrderBy(lt => lt.DisplayOrder).ToListAsync(), "Id", "Name");
    }

    private static string GenerateSlug(string name)
    {
        return name.Replace(" ", "-").Replace("‌", "-").ToLowerInvariant();
    }
}
