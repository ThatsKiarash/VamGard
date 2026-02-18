using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VamYab.Data;
using VamYab.Middleware;
using VamYab.Models;
using VamYab.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddResponseCompression();
var connStr = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=vamyab.db";
if (connStr.Contains("Data Source=vamyab.db") && !connStr.Contains(Path.DirectorySeparatorChar) && !connStr.Contains('/'))
{
    var dbPath = Path.Combine(builder.Environment.ContentRootPath, "vamyab.db");
    connStr = $"Data Source={dbPath}";
}
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connStr));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Admin/Auth/Login";
        options.LogoutPath = "/Admin/Auth/Logout";
        options.AccessDeniedPath = "/Admin/Auth/Login";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
    });

builder.Services.AddSingleton<IPasswordHasher<AdminUser>, PasswordHasher<AdminUser>>();
builder.Services.AddTransient<EmailService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();

    if (!db.AdminUsers.Any())
    {
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<AdminUser>>();
        var admin = new AdminUser
        {
            Username = "admin",
            DisplayName = "مدیر سایت",
            CreatedAt = DateTime.UtcNow
        };
        admin.PasswordHash = hasher.HashPassword(admin, "admin123");
        db.AdminUsers.Add(admin);
        db.SaveChanges();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=604800");
    }
});
app.UseResponseCompression();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<AnalyticsMiddleware>();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "loan-detail",
    pattern: "vam/{slug}",
    defaults: new { controller = "Loans", action = "Detail" });

app.MapControllerRoute(
    name: "bank-detail",
    pattern: "bank/{slug}",
    defaults: new { controller = "Banks", action = "Detail" });

app.MapControllerRoute(
    name: "loan-type",
    pattern: "type/{slug}",
    defaults: new { controller = "Loans", action = "ByType" });

app.MapControllerRoute(
    name: "blog-detail",
    pattern: "blog/{slug}",
    defaults: new { controller = "Blog", action = "Detail" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
