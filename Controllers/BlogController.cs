using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VamYab.Data;

namespace VamYab.Controllers;

public class BlogController : Controller
{
    private readonly ApplicationDbContext _db;

    public BlogController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index(int page = 1, string? category = null)
    {
        const int pageSize = 12;
        var query = _db.BlogPosts.Where(p => p.IsPublished);
        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(p => p.Category == category);
        query = query.OrderByDescending(p => p.PublishedAt);

        var categories = await _db.BlogPosts
            .Where(p => p.IsPublished && p.Category != null && p.Category != "")
            .Select(p => p.Category!)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();

        ViewBag.TotalCount = await query.CountAsync();
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling((double)ViewBag.TotalCount / pageSize);
        ViewBag.Categories = categories;
        ViewBag.CurrentCategory = category;

        var posts = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return View(posts);
    }

    public async Task<IActionResult> Detail(string slug)
    {
        var post = await _db.BlogPosts
            .FirstOrDefaultAsync(p => p.Slug == slug && p.IsPublished);

        if (post == null) return NotFound();

        post.ViewCount++;
        await _db.SaveChangesAsync();

        ViewBag.RelatedPosts = await _db.BlogPosts
            .Where(p => p.IsPublished && p.Id != post.Id)
            .OrderByDescending(p => p.PublishedAt)
            .Take(4)
            .ToListAsync();

        return View(post);
    }
}
