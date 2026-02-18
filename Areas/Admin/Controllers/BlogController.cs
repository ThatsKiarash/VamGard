using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VamYab.Data;
using VamYab.Models;

namespace VamYab.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class BlogController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IWebHostEnvironment _env;

    public BlogController(ApplicationDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    public async Task<IActionResult> Index()
    {
        var posts = await _db.BlogPosts.OrderByDescending(p => p.UpdatedAt).ToListAsync();
        return View(posts);
    }

    public IActionResult Create()
    {
        return View(new BlogPost());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BlogPost post, IFormFile? coverImage)
    {
        if (string.IsNullOrWhiteSpace(post.Slug))
            post.Slug = GenerateSlug(post.Title);

        if (await _db.BlogPosts.AnyAsync(p => p.Slug == post.Slug))
            ModelState.AddModelError("Slug", "این اسلاگ قبلاً استفاده شده است");

        if (!ModelState.IsValid)
            return View(post);

        if (coverImage != null && coverImage.Length > 0)
            post.CoverImageUrl = await SaveImage(coverImage);

        post.CreatedAt = DateTime.UtcNow;
        post.UpdatedAt = DateTime.UtcNow;
        if (post.IsPublished && post.PublishedAt == null)
            post.PublishedAt = DateTime.UtcNow;

        _db.BlogPosts.Add(post);
        await _db.SaveChangesAsync();
        TempData["Success"] = "مطلب با موفقیت ایجاد شد";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var post = await _db.BlogPosts.FindAsync(id);
        if (post == null) return NotFound();
        return View(post);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, BlogPost post, IFormFile? coverImage)
    {
        if (id != post.Id) return NotFound();

        if (string.IsNullOrWhiteSpace(post.Slug))
            post.Slug = GenerateSlug(post.Title);

        if (await _db.BlogPosts.AnyAsync(p => p.Slug == post.Slug && p.Id != id))
            ModelState.AddModelError("Slug", "این اسلاگ قبلاً استفاده شده است");

        if (!ModelState.IsValid)
            return View(post);

        var existing = await _db.BlogPosts.FindAsync(id);
        if (existing == null) return NotFound();

        if (coverImage != null && coverImage.Length > 0)
            existing.CoverImageUrl = await SaveImage(coverImage);
        else if (!string.IsNullOrEmpty(post.CoverImageUrl))
            existing.CoverImageUrl = post.CoverImageUrl;

        existing.Title = post.Title;
        existing.Slug = post.Slug;
        existing.Summary = post.Summary;
        existing.Content = post.Content;
        existing.MetaTitle = post.MetaTitle;
        existing.MetaDescription = post.MetaDescription;
        existing.MetaKeywords = post.MetaKeywords;
        existing.IsPublished = post.IsPublished;
        existing.UpdatedAt = DateTime.UtcNow;

        if (post.IsPublished && existing.PublishedAt == null)
            existing.PublishedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        TempData["Success"] = "مطلب با موفقیت ویرایش شد";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var post = await _db.BlogPosts.FindAsync(id);
        if (post == null) return NotFound();

        _db.BlogPosts.Remove(post);
        await _db.SaveChangesAsync();
        TempData["Success"] = "مطلب با موفقیت حذف شد";
        return RedirectToAction(nameof(Index));
    }

    private async Task<string> SaveImage(IFormFile file)
    {
        var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", "blog");
        Directory.CreateDirectory(uploadsDir);
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (string.IsNullOrEmpty(ext)) ext = ".jpg";
        var fileName = $"{Guid.NewGuid():N}{ext}";
        var filePath = Path.Combine(uploadsDir, fileName);
        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);
        return $"/uploads/blog/{fileName}";
    }

    private static string GenerateSlug(string name)
    {
        return name.Replace(" ", "-").Replace("‌", "-").ToLowerInvariant();
    }
}
