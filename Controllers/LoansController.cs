using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VamYab.Data;

namespace VamYab.Controllers;

public class LoansController : Controller
{
    private readonly ApplicationDbContext _db;

    public LoansController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index(string? bank, string? type, string? q)
    {
        var query = _db.Loans
            .Include(l => l.Bank)
            .Include(l => l.LoanType)
            .Where(l => l.IsActive);

        if (!string.IsNullOrWhiteSpace(bank))
            query = query.Where(l => l.Bank.Slug == bank);

        if (!string.IsNullOrWhiteSpace(type))
            query = query.Where(l => l.LoanType.Slug == type);

        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(l => l.Title.Contains(q) || (l.ShortDescription != null && l.ShortDescription.Contains(q)));

        ViewBag.Banks = await _db.Banks.Where(b => b.IsActive).OrderBy(b => b.DisplayOrder).ToListAsync();
        ViewBag.LoanTypes = await _db.LoanTypes.Where(lt => lt.IsActive).OrderBy(lt => lt.DisplayOrder).ToListAsync();
        ViewBag.SelectedBank = bank;
        ViewBag.SelectedType = type;
        ViewBag.SearchQuery = q;

        var loans = await query.OrderByDescending(l => l.IsFeatured).ThenByDescending(l => l.UpdatedAt).ToListAsync();
        return View(loans);
    }

    public async Task<IActionResult> Detail(string slug)
    {
        var loan = await _db.Loans
            .Include(l => l.Bank)
            .Include(l => l.LoanType)
            .FirstOrDefaultAsync(l => l.Slug == slug && l.IsActive);

        if (loan == null)
            return NotFound();

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var path = $"/vam/{slug}";
        var oneDayAgo = DateTime.UtcNow.AddDays(-1);
        var alreadyVisited = !string.IsNullOrEmpty(ip) && await _db.PageVisits
            .AnyAsync(v => v.Path == path && v.IpAddress == ip && v.VisitedAt >= oneDayAgo);

        if (!alreadyVisited)
        {
            loan.ViewCount++;
            await _db.SaveChangesAsync();
        }

        ViewBag.RelatedLoans = await _db.Loans
            .Include(l => l.Bank)
            .Where(l => l.IsActive && l.LoanTypeId == loan.LoanTypeId && l.Id != loan.Id)
            .Take(4)
            .ToListAsync();

        ViewBag.SameBankLoans = await _db.Loans
            .Include(l => l.LoanType)
            .Where(l => l.IsActive && l.BankId == loan.BankId && l.Id != loan.Id)
            .Take(4)
            .ToListAsync();

        var relatedArticles = new List<VamYab.Models.BlogPost>();
        if (!string.IsNullOrEmpty(loan.RelatedArticleIds))
        {
            var articleIds = loan.RelatedArticleIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.TryParse(s.Trim(), out var id) ? id : 0).Where(id => id > 0).ToList();
            if (articleIds.Any())
                relatedArticles = await _db.BlogPosts.Where(p => articleIds.Contains(p.Id) && p.IsPublished).ToListAsync();
        }
        if (!relatedArticles.Any())
        {
            relatedArticles = await _db.BlogPosts
                .Where(p => p.IsPublished && (p.RelatedLoanId == loan.Id ||
                    (p.RelatedLoanIds != null && p.RelatedLoanIds.Contains(loan.Id.ToString()))))
                .Take(5).ToListAsync();
        }
        ViewBag.RelatedArticles = relatedArticles;

        return View(loan);
    }

    public async Task<IActionResult> ByType(string slug)
    {
        var loanType = await _db.LoanTypes.FirstOrDefaultAsync(lt => lt.Slug == slug && lt.IsActive);
        if (loanType == null)
            return NotFound();

        var loans = await _db.Loans
            .Include(l => l.Bank)
            .Include(l => l.LoanType)
            .Where(l => l.IsActive && l.LoanTypeId == loanType.Id)
            .OrderByDescending(l => l.IsFeatured)
            .ThenByDescending(l => l.UpdatedAt)
            .ToListAsync();

        ViewBag.LoanType = loanType;
        return View(loans);
    }
}
