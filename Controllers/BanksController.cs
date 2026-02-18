using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VamYab.Data;

namespace VamYab.Controllers;

public class BanksController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IHttpClientFactory _httpFactory;

    public BanksController(ApplicationDbContext db, IHttpClientFactory httpFactory)
    {
        _db = db;
        _httpFactory = httpFactory;
    }

    public async Task<IActionResult> Index()
    {
        var banks = await _db.Banks
            .Where(b => b.IsActive)
            .OrderBy(b => b.DisplayOrder)
            .ToListAsync();

        foreach (var bank in banks)
        {
            bank.Loans = await _db.Loans
                .Where(l => l.BankId == bank.Id && l.IsActive)
                .ToListAsync();
        }

        return View(banks);
    }

    public async Task<IActionResult> Detail(string slug)
    {
        var bank = await _db.Banks
            .FirstOrDefaultAsync(b => b.Slug == slug && b.IsActive);

        if (bank == null)
            return NotFound();

        ViewBag.Loans = await _db.Loans
            .Include(l => l.LoanType)
            .Where(l => l.BankId == bank.Id && l.IsActive)
            .OrderByDescending(l => l.IsFeatured)
            .ThenByDescending(l => l.UpdatedAt)
            .ToListAsync();

        return View(bank);
    }

    [HttpGet("/api/banks/{slug}/nearby")]
    public async Task<IActionResult> NearbyBranches(string slug, double lat, double lng)
    {
        var bank = await _db.Banks.FirstOrDefaultAsync(b => b.Slug == slug);
        if (bank == null) return NotFound();

        try
        {
            var bankName = bank.Name.Replace("بانک ", "").Replace("قرض‌الحسنه ", "").Split(' ')[0];
            var query = $@"[out:json][timeout:10];
(
  node[""amenity""=""bank""][""name""~""{bankName}""](around:5000,{lat.ToString(System.Globalization.CultureInfo.InvariantCulture)},{lng.ToString(System.Globalization.CultureInfo.InvariantCulture)});
  way[""amenity""=""bank""][""name""~""{bankName}""](around:5000,{lat.ToString(System.Globalization.CultureInfo.InvariantCulture)},{lng.ToString(System.Globalization.CultureInfo.InvariantCulture)});
);
out center body;";

            var client = _httpFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(15);
            var response = await client.PostAsync(
                "https://overpass-api.de/api/interpreter",
                new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("data", query) })
            );

            if (!response.IsSuccessStatusCode)
                return Json(new { success = false, message = "خطا در ارتباط با سرور نقشه" });

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var elements = doc.RootElement.GetProperty("elements");

            var branches = new List<object>();
            foreach (var el in elements.EnumerateArray())
            {
                double? elLat = null, elLng = null;
                if (el.TryGetProperty("lat", out var latProp))
                    elLat = latProp.GetDouble();
                else if (el.TryGetProperty("center", out var center))
                {
                    elLat = center.GetProperty("lat").GetDouble();
                    elLng = center.GetProperty("lon").GetDouble();
                }
                if (el.TryGetProperty("lon", out var lonProp))
                    elLng = lonProp.GetDouble();

                var tags = el.TryGetProperty("tags", out var t) ? t : default;
                var name = tags.ValueKind != JsonValueKind.Undefined && tags.TryGetProperty("name", out var n) ? n.GetString() : bank.Name;
                var addr = tags.ValueKind != JsonValueKind.Undefined && tags.TryGetProperty("addr:street", out var a) ? a.GetString() : null;

                if (elLat.HasValue && elLng.HasValue)
                    branches.Add(new { name, lat = elLat.Value, lng = elLng.Value, address = addr });
            }

            return Json(new { success = true, branches, count = branches.Count });
        }
        catch
        {
            return Json(new { success = false, message = "خطا در جستجوی شعب" });
        }
    }
}
