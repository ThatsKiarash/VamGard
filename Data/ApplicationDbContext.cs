using Microsoft.EntityFrameworkCore;
using VamYab.Models;

namespace VamYab.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Bank> Banks => Set<Bank>();
    public DbSet<LoanType> LoanTypes => Set<LoanType>();
    public DbSet<Loan> Loans => Set<Loan>();
    public DbSet<NewsletterSubscriber> NewsletterSubscribers => Set<NewsletterSubscriber>();
    public DbSet<PageVisit> PageVisits => Set<PageVisit>();
    public DbSet<BlogPost> BlogPosts => Set<BlogPost>();
    public DbSet<AdminUser> AdminUsers => Set<AdminUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Bank>(entity =>
        {
            entity.HasIndex(e => e.Slug).IsUnique();
            entity.HasIndex(e => e.IsActive);
        });

        modelBuilder.Entity<NewsletterSubscriber>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique();
        });

        modelBuilder.Entity<LoanType>(entity =>
        {
            entity.HasIndex(e => e.Slug).IsUnique();
            entity.HasIndex(e => e.IsActive);
        });

        modelBuilder.Entity<Loan>(entity =>
        {
            entity.HasIndex(e => e.Slug).IsUnique();
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.IsFeatured);
            entity.HasIndex(e => new { e.BankId, e.LoanTypeId });

            entity.HasOne(e => e.Bank)
                .WithMany(b => b.Loans)
                .HasForeignKey(e => e.BankId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.LoanType)
                .WithMany(lt => lt.Loans)
                .HasForeignKey(e => e.LoanTypeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PageVisit>(entity =>
        {
            entity.HasIndex(e => e.Path);
            entity.HasIndex(e => e.IpAddress);
            entity.HasIndex(e => e.VisitedAt);
        });

        modelBuilder.Entity<BlogPost>(entity =>
        {
            entity.HasIndex(e => e.Slug).IsUnique();
            entity.HasIndex(e => e.IsPublished);
        });

        modelBuilder.Entity<AdminUser>(entity =>
        {
            entity.HasIndex(e => e.Username).IsUnique();
        });

        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        var logo = "https://raw.githubusercontent.com/amastaneh/IranianBankLogos/master/resources/images/";
        var neo = "https://raw.githubusercontent.com/snapp-store/iranian-banks-react-icons/main/raw/";
        var d = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // ══════════════════════════════════════════════════════════════════
        //  BANKS (1-20) + NEOBANKS (21-34)
        // ══════════════════════════════════════════════════════════════════
        modelBuilder.Entity<Bank>().HasData(
            // ── 1. بانک ملی ایران ──
            new Bank
            {
                Id = 1, Name = "بانک ملی ایران", Slug = "bank-melli", LogoUrl = logo + "bmi-300-wc.png",
                Website = "https://www.bmi.ir", DisplayOrder = 1, IsActive = true, CreatedAt = d,
                BankType = "bank", OwnershipType = "دولتی",
                FoundedYear = 1307, BranchCount = 3328, HeadquartersCity = "تهران",
                Latitude = 35.6892, Longitude = 51.3890,
                Description = "بانک ملی ایران بزرگ‌ترین بانک دولتی ایران و یکی از بزرگ‌ترین بانک‌های خاورمیانه است.",
                History = "بانک ملی ایران در ۲۰ شهریور ۱۳۰۷ با هدف حفظ استقلال مالی کشور تأسیس شد. این بانک نخستین بانک ایرانی بود که بدون سرمایه‌گذاری خارجی و صرفاً با سرمایه ملی ایجاد گردید. تا سال ۱۳۳۹ وظیفه انتشار اسکناس بر عهده این بانک بود. بانک ملی با بیش از ۳٬۳۰۰ شعبه داخلی و ۱۴ شعبه خارجی، گسترده‌ترین شبکه بانکی کشور را در اختیار دارد."
            },
            // ── 2. بانک ملت ──
            new Bank
            {
                Id = 2, Name = "بانک ملت", Slug = "bank-mellat", LogoUrl = logo + "mellat-300-wc.png",
                Website = "https://www.bankmellat.ir", DisplayOrder = 2, IsActive = true, CreatedAt = d,
                BankType = "bank", OwnershipType = "نیمه‌دولتی",
                FoundedYear = 1359, BranchCount = 1800, HeadquartersCity = "تهران",
                Latitude = 35.7219, Longitude = 51.3347,
                Description = "بانک ملت از بزرگ‌ترین بانک‌های خصوصی ایران و فعال در بورس اوراق بهادار تهران است.",
                History = "بانک ملت در سال ۱۳۵۹ از ادغام ده بانک خصوصی شامل بانک تهران، بانک داریوش، بانک پارس، بانک ایران و عرب و چند بانک دیگر تشکیل شد. این بانک در سال ۱۳۸۷ خصوصی‌سازی شد و سهام آن در بورس تهران عرضه گردید. بانک ملت جزو نخستین بانک‌های ایرانی بود که خدمات بانکداری الکترونیک را ارائه داد."
            },
            // ── 3. بانک صادرات ایران ──
            new Bank
            {
                Id = 3, Name = "بانک صادرات ایران", Slug = "bank-saderat", LogoUrl = logo + "bsi-300-wc.png",
                Website = "https://www.bsi.ir", DisplayOrder = 3, IsActive = true, CreatedAt = d,
                BankType = "bank", OwnershipType = "نیمه‌دولتی",
                FoundedYear = 1331, BranchCount = 2200, HeadquartersCity = "تهران",
                Latitude = 35.6980, Longitude = 51.4100,
                Description = "بانک صادرات ایران یکی از قدیمی‌ترین و بزرگ‌ترین بانک‌های کشور با شبکه گسترده بین‌المللی است.",
                History = "بانک صادرات ایران در سال ۱۳۳۱ با نام «بانک صادرات و معادن ایران» تأسیس شد. این بانک با هدف حمایت از صادرات غیرنفتی و توسعه صنایع معدنی کشور فعالیت خود را آغاز کرد. پس از انقلاب اسلامی ملی شد و در سال‌های اخیر فرآیند خصوصی‌سازی آن آغاز گردیده است."
            },
            // ── 4. بانک تجارت ──
            new Bank
            {
                Id = 4, Name = "بانک تجارت", Slug = "bank-tejarat", LogoUrl = logo + "tejarat-300-wc.png",
                Website = "https://www.tejaratbank.ir", DisplayOrder = 4, IsActive = true, CreatedAt = d,
                BankType = "bank", OwnershipType = "نیمه‌دولتی",
                FoundedYear = 1358, BranchCount = 2000, HeadquartersCity = "تهران",
                Latitude = 35.7000, Longitude = 51.4200,
                Description = "بانک تجارت یکی از بانک‌های بزرگ دولتی ایران با تمرکز بر خدمات تجاری و بازرگانی است.",
                History = "بانک تجارت در سال ۱۳۵۸ از ادغام یازده بانک خصوصی تشکیل شد. این بانک با تمرکز بر حوزه بازرگانی و تجارت خارجی، نقش مهمی در تسهیل مبادلات تجاری کشور ایفا می‌کند."
            },
            // ── 5. بانک سپه ──
            new Bank
            {
                Id = 5, Name = "بانک سپه", Slug = "bank-sepah", LogoUrl = logo + "sepah-300-wc.png",
                Website = "https://www.banksepah.ir", DisplayOrder = 5, IsActive = true, CreatedAt = d,
                BankType = "bank", OwnershipType = "دولتی",
                FoundedYear = 1304, BranchCount = 1700, HeadquartersCity = "تهران",
                Latitude = 35.6850, Longitude = 51.3950,
                Description = "بانک سپه قدیمی‌ترین بانک ایرانی و یکی از بزرگ‌ترین بانک‌های دولتی کشور است.",
                History = "بانک سپه در ۲۶ مهر ۱۳۰۴ خورشیدی به عنوان نخستین بانک ایرانی تأسیس شد. این بانک ابتدا با نام «بانک پهلوی قشون» فعالیت خود را آغاز کرد. بانک سپه در سال ۱۳۹۸ با ادغام بانک‌های انصار، حکمت ایرانیان، مهر اقتصاد، قوامین و کوثر بزرگ‌ترین ادغام بانکی تاریخ ایران را رقم زد."
            },
            // ── 6. بانک پاسارگاد ──
            new Bank
            {
                Id = 6, Name = "بانک پاسارگاد", Slug = "bank-pasargad", LogoUrl = logo + "bpi-300-wc.png",
                Website = "https://www.bpi.ir", DisplayOrder = 6, IsActive = true, CreatedAt = d,
                BankType = "bank", OwnershipType = "خصوصی",
                FoundedYear = 1384, BranchCount = 400, HeadquartersCity = "تهران",
                Latitude = 35.7616, Longitude = 51.4099,
                Description = "بانک پاسارگاد یکی از معتبرترین بانک‌های خصوصی ایران و چندین سال متوالی بهترین بانک ایران از نظر مجلات بین‌المللی است.",
                History = "بانک پاسارگاد در سال ۱۳۸۴ با سرمایه‌گذاری بخش خصوصی تأسیس شد. این بانک با تمرکز بر بانکداری نوین و فناوری‌های مالی، توانسته جایگاه ویژه‌ای کسب کند. چندین سال متوالی عنوان «بهترین بانک ایران» را از مجله The Banker دریافت کرده است."
            },
            // ── 7. بانک پارسیان ──
            new Bank
            {
                Id = 7, Name = "بانک پارسیان", Slug = "bank-parsian", LogoUrl = logo + "parsian-300-wc.png",
                Website = "https://www.parsian-bank.ir", DisplayOrder = 7, IsActive = true, CreatedAt = d,
                BankType = "bank", OwnershipType = "خصوصی",
                FoundedYear = 1381, BranchCount = 350, HeadquartersCity = "تهران",
                Latitude = 35.7450, Longitude = 51.3750,
                Description = "بانک پارسیان از بانک‌های خصوصی پیشرو در ایران با خدمات متنوع بانکداری الکترونیک است.",
                History = "بانک پارسیان در سال ۱۳۸۱ به عنوان یکی از نخستین بانک‌های خصوصی پس از انقلاب اسلامی تأسیس شد. این بانک با سرمایه‌گذاری شرکت‌های بزرگ بخش خصوصی تشکیل گردید و همواره پیشگام در ارائه خدمات نوین بانکی بوده است."
            },
            // ── 8. بانک مسکن ──
            new Bank
            {
                Id = 8, Name = "بانک مسکن", Slug = "bank-maskan", LogoUrl = logo + "maskan-300-wc.png",
                Website = "https://www.bank-maskan.ir", DisplayOrder = 8, IsActive = true, CreatedAt = d,
                BankType = "bank", OwnershipType = "دولتی",
                FoundedYear = 1317, BranchCount = 1300, HeadquartersCity = "تهران",
                Latitude = 35.7150, Longitude = 51.4050,
                Description = "بانک مسکن تنها بانک تخصصی حوزه مسکن در ایران و مهم‌ترین نهاد تأمین مالی بخش مسکن کشور است.",
                History = "بانک مسکن در سال ۱۳۱۷ با نام «بانک رهنی ایران» تأسیس شد و در سال ۱۳۵۸ به «بانک مسکن» تغییر نام داد. طرح‌های بزرگی مانند مسکن مهر و نهضت ملی مسکن از طریق این بانک اجرا شده‌اند."
            },
            // ── 9. بانک کشاورزی ──
            new Bank
            {
                Id = 9, Name = "بانک کشاورزی", Slug = "bank-keshavarzi", LogoUrl = logo + "bki-300-wc.png",
                Website = "https://www.bki.ir", DisplayOrder = 9, IsActive = true, CreatedAt = d,
                BankType = "bank", OwnershipType = "دولتی",
                FoundedYear = 1312, BranchCount = 1800, HeadquartersCity = "تهران",
                Latitude = 35.7100, Longitude = 51.4000,
                Description = "بانک کشاورزی تنها بانک تخصصی بخش کشاورزی ایران و حامی اصلی توسعه روستایی و کشاورزی است.",
                History = "بانک کشاورزی در سال ۱۳۱۲ با نام «بانک فلاحتی و صنعتی» تأسیس شد. این بانک گسترده‌ترین شبکه بانکی را در مناطق روستایی کشور دارد و نقش کلیدی در تأمین مالی طرح‌های کشاورزی ایفا می‌کند."
            },
            // ── 10. بانک رفاه کارگران ──
            new Bank
            {
                Id = 10, Name = "بانک رفاه کارگران", Slug = "bank-refah", LogoUrl = logo + "rb-300-wc.png",
                Website = "https://www.refah-bank.ir", DisplayOrder = 10, IsActive = true, CreatedAt = d,
                BankType = "bank", OwnershipType = "نیمه‌دولتی",
                FoundedYear = 1339, BranchCount = 1000, HeadquartersCity = "تهران",
                Latitude = 35.7200, Longitude = 51.3900,
                Description = "بانک رفاه کارگران بانک تخصصی حوزه رفاه اجتماعی و خدمات به اقشار حقوق‌بگیر و بازنشسته است.",
                History = "بانک رفاه کارگران در سال ۱۳۳۹ با هدف ارائه خدمات بانکی به کارگران و حقوق‌بگیران تأسیس شد. این بانک زیرمجموعه سازمان تأمین اجتماعی است و بخش عمده‌ای از پرداخت حقوق بازنشستگان را انجام می‌دهد."
            },
            // ── 11. پست بانک ایران ──
            new Bank
            {
                Id = 11, Name = "پست بانک ایران", Slug = "post-bank", LogoUrl = logo + "post-300-wc.png",
                Website = "https://www.postbank.ir", DisplayOrder = 11, IsActive = true, CreatedAt = d,
                BankType = "bank", OwnershipType = "دولتی",
                FoundedYear = 1375, BranchCount = 1000, HeadquartersCity = "تهران",
                Latitude = 35.7001, Longitude = 51.4230,
                Description = "پست بانک ایران بانک دولتی تخصصی توسعه روستایی و ارتباطات مالی مناطق محروم کشور است.",
                History = "پست بانک ایران در سال ۱۳۷۵ با هدف توسعه خدمات بانکی در مناطق روستایی و محروم تأسیس شد. این بانک با استفاده از شبکه گسترده دفاتر پستی، خدمات بانکی را به نقاط دوردست کشور ارائه می‌دهد. پست بانک بیش از ۱۱ هزار نقطه دسترسی در سراسر ایران دارد و نقش مهمی در پرداخت یارانه‌ها و مستمری‌های دولتی ایفا می‌کند."
            },
            // ── 12. بانک سامان ──
            new Bank
            {
                Id = 12, Name = "بانک سامان", Slug = "bank-saman", LogoUrl = logo + "sb-300-wc.png",
                Website = "https://www.sb24.ir", DisplayOrder = 12, IsActive = true, CreatedAt = d,
                BankType = "bank", OwnershipType = "خصوصی",
                FoundedYear = 1381, BranchCount = 250, HeadquartersCity = "تهران",
                Latitude = 35.7860, Longitude = 51.4140,
                Description = "بانک سامان از بانک‌های خصوصی پیشرو با تمرکز ویژه بر بانکداری دیجیتال و فناوری مالی است.",
                History = "بانک سامان در سال ۱۳۸۱ تأسیس شد و از نخستین بانک‌های خصوصی ایران پس از انقلاب اسلامی محسوب می‌شود. این بانک پیشگام در حوزه بانکداری دیجیتال بوده و ترابانک «بلو» (Blu) را راه‌اندازی کرده که یکی از موفق‌ترین نئوبانک‌های ایران است. بانک سامان همچنین در حوزه پرداخت الکترونیک و درگاه‌های پرداخت اینترنتی نقش مهمی دارد."
            },
            // ── 13. بانک اقتصاد نوین ──
            new Bank
            {
                Id = 13, Name = "بانک اقتصاد نوین", Slug = "bank-eghtesad-novin", LogoUrl = logo + "en-300-wc.png",
                Website = "https://www.enbank.ir", DisplayOrder = 13, IsActive = true, CreatedAt = d,
                BankType = "bank", OwnershipType = "خصوصی",
                FoundedYear = 1380, BranchCount = 300, HeadquartersCity = "تهران",
                Latitude = 35.7520, Longitude = 51.4310,
                Description = "بانک اقتصاد نوین نخستین بانک خصوصی ایران پس از انقلاب اسلامی و پیشگام در نوآوری بانکی است.",
                History = "بانک اقتصاد نوین در سال ۱۳۸۰ به عنوان اولین بانک خصوصی پس از انقلاب اسلامی مجوز فعالیت دریافت کرد. این بانک با شعار «بانکداری نوین» از ابتدا بر فناوری و خدمات الکترونیک تمرکز داشته و در توسعه بانکداری مدرن در ایران نقش پیشرو ایفا کرده است."
            },
            // ── 14. بانک کارآفرین ──
            new Bank
            {
                Id = 14, Name = "بانک کارآفرین", Slug = "bank-karafarin", LogoUrl = logo + "kar-300-wc.png",
                Website = "https://www.karafarinbank.ir", DisplayOrder = 14, IsActive = true, CreatedAt = d,
                BankType = "bank", OwnershipType = "خصوصی",
                FoundedYear = 1378, BranchCount = 80, HeadquartersCity = "تهران",
                Latitude = 35.7590, Longitude = 51.4190,
                Description = "بانک کارآفرین بانک خصوصی با تمرکز بر تأمین مالی بنگاه‌های اقتصادی و کارآفرینان است.",
                History = "بانک کارآفرین در سال ۱۳۷۸ با نام مؤسسه اعتباری کارآفرین فعالیت خود را آغاز کرد و در سال ۱۳۸۳ مجوز فعالیت بانکی دریافت نمود. این بانک با تمرکز بر تسهیلات بنگاه‌های کوچک و متوسط (SME) و کارآفرینان، جایگاه ویژه‌ای در بازار بانکی ایران یافته است. بانک کارآفرین رویکرد تخصصی در حوزه بانکداری شرکتی و تأمین مالی پروژه‌ها دارد."
            },
            // ── 15. بانک سینا ──
            new Bank
            {
                Id = 15, Name = "بانک سینا", Slug = "bank-sina", LogoUrl = logo + "sina-300-wc.png",
                Website = "https://www.sinabank.ir", DisplayOrder = 15, IsActive = true, CreatedAt = d,
                BankType = "bank", OwnershipType = "خصوصی",
                FoundedYear = 1387, BranchCount = 220, HeadquartersCity = "تهران",
                Latitude = 35.7310, Longitude = 51.3870,
                Description = "بانک سینا از بانک‌های خصوصی فعال در بورس تهران با خدمات متنوع بانکداری خرد و شرکتی است.",
                History = "بانک سینا در سال ۱۳۸۷ از ادغام شرکت‌های لیزینگ و مؤسسات مالی وابسته به بنیاد مستضعفان تأسیس شد. این بانک سهام‌دار عمده‌اش بنیاد مستضعفان و جانبازان انقلاب اسلامی است. بانک سینا ترابانک «سیبانک» را برای ارائه خدمات دیجیتال راه‌اندازی کرده است."
            },
            // ── 16. بانک شهر ──
            new Bank
            {
                Id = 16, Name = "بانک شهر", Slug = "bank-shahr", LogoUrl = logo + "shahr-300-wc.png",
                Website = "https://www.shahr-bank.ir", DisplayOrder = 16, IsActive = true, CreatedAt = d,
                BankType = "bank", OwnershipType = "خصوصی",
                FoundedYear = 1387, BranchCount = 310, HeadquartersCity = "تهران",
                Latitude = 35.7120, Longitude = 51.3980,
                Description = "بانک شهر بانک خصوصی وابسته به شهرداری‌های کشور با تمرکز بر خدمات شهری و عمرانی است.",
                History = "بانک شهر در سال ۱۳۸۷ با سرمایه‌گذاری شهرداری تهران و شهرداری‌های کلانشهرها تأسیس شد. این بانک با هدف تأمین مالی پروژه‌های عمرانی و شهری فعالیت خود را آغاز کرد. بانک شهر در حوزه پرداخت عوارض شهری، قبوض و خدمات شهروندی نقش فعالی دارد و ترابانک «همراه‌شهر پلاس» را راه‌اندازی کرده است."
            },
            // ── 17. بانک گردشگری ──
            new Bank
            {
                Id = 17, Name = "بانک گردشگری", Slug = "bank-gardeshgari", LogoUrl = logo + "tourism-300-wc.png",
                Website = "https://www.tourismbank.ir", DisplayOrder = 17, IsActive = true, CreatedAt = d,
                BankType = "bank", OwnershipType = "خصوصی",
                FoundedYear = 1389, BranchCount = 65, HeadquartersCity = "تهران",
                Latitude = 35.7760, Longitude = 51.4260,
                Description = "بانک گردشگری تنها بانک تخصصی حوزه گردشگری و هتلداری ایران و فعال در تأمین مالی صنعت توریسم است.",
                History = "بانک گردشگری در سال ۱۳۸۹ با هدف حمایت از صنعت گردشگری و هتلداری ایران تأسیس شد. این بانک با ارائه تسهیلات ویژه به هتل‌ها، آژانس‌های مسافرتی و فعالان حوزه گردشگری، نقش مهمی در توسعه زیرساخت‌های توریستی کشور ایفا می‌کند. ترابانک «ایوا» (نشان‌بانک) توسط این بانک راه‌اندازی شده است."
            },
            // ── 18. بانک خاورمیانه ──
            new Bank
            {
                Id = 18, Name = "بانک خاورمیانه", Slug = "bank-khavarmianeh", LogoUrl = logo + "me-300-wc.png",
                Website = "https://www.mebank.ir", DisplayOrder = 18, IsActive = true, CreatedAt = d,
                BankType = "bank", OwnershipType = "خصوصی",
                FoundedYear = 1389, BranchCount = 15, HeadquartersCity = "تهران",
                Latitude = 35.7730, Longitude = 51.4170,
                Description = "بانک خاورمیانه بانک خصوصی کوچک اما نوآور با تمرکز بر بانکداری دیجیتال و مدرن است.",
                History = "بانک خاورمیانه در سال ۱۳۸۹ تأسیس شد و با وجود تعداد شعب کم، با راه‌اندازی ترابانک «بانکینو» به یکی از پیشگامان بانکداری دیجیتال ایران تبدیل شده است. بانک خاورمیانه رویکرد تخصصی در حوزه فناوری مالی دارد و خدمات نوین بانکی را از طریق پلتفرم‌های دیجیتال ارائه می‌دهد."
            },
            // ── 19. بانک قرض‌الحسنه مهر ایران ──
            new Bank
            {
                Id = 19, Name = "بانک قرض‌الحسنه مهر ایران", Slug = "bank-mehr", LogoUrl = logo + "mehriran-300-wc.png",
                Website = "https://www.qmb.ir", DisplayOrder = 19, IsActive = true, CreatedAt = d,
                BankType = "bank", OwnershipType = "نیمه‌دولتی",
                FoundedYear = 1386, BranchCount = 900, HeadquartersCity = "تهران",
                Latitude = 35.7040, Longitude = 51.4020,
                Description = "بانک قرض‌الحسنه مهر ایران بزرگ‌ترین بانک قرض‌الحسنه‌ای کشور با تمرکز بر وام‌های بدون بهره است.",
                History = "بانک قرض‌الحسنه مهر ایران در سال ۱۳۸۶ با ادغام صندوق‌های قرض‌الحسنه نیروهای مسلح تأسیس شد. این بانک بزرگ‌ترین مؤسسه قرض‌الحسنه‌ای جهان محسوب می‌شود و تسهیلات بدون بهره و کم‌بهره به اقشار نیازمند ارائه می‌دهد. بانک مهر ایران دو ترابانک «بانکت» و «امیدبانک» را راه‌اندازی کرده است."
            },
            // ── 20. بانک قرض‌الحسنه رسالت ──
            new Bank
            {
                Id = 20, Name = "بانک قرض‌الحسنه رسالت", Slug = "bank-resalat", LogoUrl = logo + "resalat-300-wc.png",
                Website = "https://www.resalatbank.ir", DisplayOrder = 20, IsActive = true, CreatedAt = d,
                BankType = "bank", OwnershipType = "سهامی عام",
                FoundedYear = 1391, BranchCount = 420, HeadquartersCity = "تهران",
                Latitude = 35.6950, Longitude = 51.4150,
                Description = "بانک قرض‌الحسنه رسالت بانک تعاونی مردم‌محور با تمرکز بر وام‌های خرد و قرض‌الحسنه است.",
                History = "بانک قرض‌الحسنه رسالت در سال ۱۳۹۱ با پایه‌گذاری از صندوق‌های قرض‌الحسنه مردمی تأسیس شد. این بانک با رویکرد تعاونی و مردم‌محور، تسهیلات قرض‌الحسنه و خرد را به اقشار کم‌درآمد و نیازمند جامعه ارائه می‌دهد. بانک رسالت سهام خود را در بورس تهران عرضه کرده و سهام‌داران متعددی از آحاد مردم دارد."
            },

            // ══════════════════════════════════════════════════════════════
            //  NEOBANKS (ترابانک‌ها)
            // ══════════════════════════════════════════════════════════════

            // ── 21. هپی (HiBank) ──
            new Bank
            {
                Id = 21, Name = "هپی (HiBank)", Slug = "hibank", LogoUrl = logo + "hi-300-wc.png",
                Website = "https://hibank.ir", DisplayOrder = 21, IsActive = true, CreatedAt = d,
                BankType = "neobank", ParentBankId = 6, OwnershipType = "ترابانک",
                FoundedYear = 1399, HeadquartersCity = "تهران",
                Description = "هپی (HiBank) ترابانک بانک پاسارگاد و از پیشگامان بانکداری دیجیتال ایران است.",
                History = "هپی در سال ۱۳۹۹ توسط بانک پاسارگاد راه‌اندازی شد و یکی از نخستین نئوبانک‌های ایران محسوب می‌شود. این اپلیکیشن امکان افتتاح حساب آنلاین، انتقال وجه، مدیریت کارت و سرمایه‌گذاری را فراهم می‌کند."
            },
            // ── 22. ویپاد ──
            new Bank
            {
                Id = 22, Name = "ویپاد", Slug = "vipad", LogoUrl = logo + "bpi-300-wc.png",
                Website = "https://www.vipad.ir", DisplayOrder = 22, IsActive = true, CreatedAt = d,
                BankType = "neobank", ParentBankId = 6, OwnershipType = "ترابانک",
                FoundedYear = 1401, HeadquartersCity = "تهران",
                Description = "ویپاد ترابانک بانک پاسارگاد با تمرکز بر خدمات پرداخت و کیف پول دیجیتال است.",
                History = "ویپاد در سال ۱۴۰۱ توسط بانک پاسارگاد به عنوان دومین ترابانک این مجموعه راه‌اندازی شد. ویپاد بر ارائه خدمات پرداخت ساده، کیف پول الکترونیکی و تراکنش‌های روزمره تمرکز دارد."
            },
            // ── 23. بلو (بلوبانک) ──
            new Bank
            {
                Id = 23, Name = "بلو (بلوبانک)", Slug = "blubank", LogoUrl = neo + "blu-bank-color.svg",
                Website = "https://blubank.com", DisplayOrder = 23, IsActive = true, CreatedAt = d,
                BankType = "neobank", ParentBankId = 12, OwnershipType = "ترابانک",
                FoundedYear = 1399, HeadquartersCity = "تهران",
                Description = "بلوبانک محبوب‌ترین ترابانک ایران و وابسته به بانک سامان با میلیون‌ها کاربر فعال است.",
                History = "بلو (Blu) در سال ۱۳۹۹ توسط بانک سامان راه‌اندازی شد و به سرعت به محبوب‌ترین نئوبانک ایران تبدیل گردید. بلوبانک با رابط کاربری ساده و جذاب، امکان افتتاح حساب غیرحضوری، صدور کارت مجازی و فیزیکی، قابلیت پس‌انداز هوشمند و باشگاه مشتریان را ارائه می‌دهد. بلو بیش از ۱۰ میلیون کاربر فعال دارد."
            },
            // ── 24. سپینو ──
            new Bank
            {
                Id = 24, Name = "سپینو", Slug = "sepino", LogoUrl = logo + "bsi-300-wc.png",
                Website = "https://sepino.ir", DisplayOrder = 24, IsActive = true, CreatedAt = d,
                BankType = "neobank", ParentBankId = 3, OwnershipType = "ترابانک",
                FoundedYear = 1401, HeadquartersCity = "تهران",
                Description = "سپینو ترابانک بانک صادرات ایران با خدمات بانکداری دیجیتال نوین و کاربرپسند است.",
                History = "سپینو در سال ۱۴۰۱ توسط بانک صادرات ایران راه‌اندازی شد. این ترابانک خدماتی شامل افتتاح حساب آنلاین، انتقال وجه، پرداخت قبوض، خرید شارژ و مدیریت مالی شخصی ارائه می‌دهد."
            },
            // ── 25. بانکینو ──
            new Bank
            {
                Id = 25, Name = "بانکینو", Slug = "bankino", LogoUrl = neo + "bankino-color.svg",
                Website = "https://bankino.ir", DisplayOrder = 25, IsActive = true, CreatedAt = d,
                BankType = "neobank", ParentBankId = 18, OwnershipType = "ترابانک",
                FoundedYear = 1399, HeadquartersCity = "تهران",
                Description = "بانکینو ترابانک بانک خاورمیانه و یکی از نخستین نئوبانک‌های ایران با تجربه کاربری مدرن است.",
                History = "بانکینو در سال ۱۳۹۹ توسط بانک خاورمیانه راه‌اندازی شد و یکی از نخستین ترابانک‌های ایران محسوب می‌شود. بانکینو با طراحی مینیمال و تجربه کاربری مدرن، خدمات افتتاح حساب دیجیتال، کارت بانکی، انتقال وجه و مدیریت هزینه‌ها را ارائه می‌دهد."
            },
            // ── 26. ایوا (نشان‌بانک) ──
            new Bank
            {
                Id = 26, Name = "ایوا (نشان‌بانک)", Slug = "eva-bank", LogoUrl = logo + "tourism-300-wc.png",
                Website = "https://neshanbank.ir", DisplayOrder = 26, IsActive = true, CreatedAt = d,
                BankType = "neobank", ParentBankId = 17, OwnershipType = "ترابانک",
                FoundedYear = 1402, HeadquartersCity = "تهران",
                Description = "ایوا ترابانک بانک گردشگری با خدمات بانکداری دیجیتال و تمرکز بر سفر و گردشگری است.",
                History = "ایوا (Eva) در سال ۱۴۰۲ توسط بانک گردشگری با نام «نشان‌بانک» راه‌اندازی شد. این ترابانک علاوه بر خدمات بانکداری دیجیتال، خدمات ویژه‌ای برای مسافران و گردشگران ارائه می‌دهد."
            },
            // ── 27. همراه‌شهر پلاس ──
            new Bank
            {
                Id = 27, Name = "همراه‌شهر پلاس", Slug = "hamrah-shahr-plus", LogoUrl = logo + "shahr-300-wc.png",
                Website = "https://hamrahshahrplus.ir", DisplayOrder = 27, IsActive = true, CreatedAt = d,
                BankType = "neobank", ParentBankId = 16, OwnershipType = "ترابانک",
                FoundedYear = 1402, HeadquartersCity = "تهران",
                Description = "همراه‌شهر پلاس ترابانک بانک شهر با خدمات بانکداری و پرداخت‌های شهری یکپارچه است.",
                History = "همراه‌شهر پلاس در سال ۱۴۰۲ توسط بانک شهر راه‌اندازی شد. این ترابانک خدمات بانکداری دیجیتال را با سرویس‌های شهری مانند پرداخت عوارض، قبوض شهرداری و خدمات حمل‌ونقل عمومی ترکیب کرده است."
            },
            // ── 28. امیدبانک ──
            new Bank
            {
                Id = 28, Name = "امیدبانک", Slug = "omidbank", LogoUrl = logo + "mehriran-300-wc.png",
                Website = "https://omidbank.ir", DisplayOrder = 28, IsActive = true, CreatedAt = d,
                BankType = "neobank", ParentBankId = 19, OwnershipType = "ترابانک",
                FoundedYear = 1403, HeadquartersCity = "تهران",
                Description = "امیدبانک جدیدترین ترابانک بانک قرض‌الحسنه مهر ایران با خدمات دیجیتال قرض‌الحسنه‌ای است.",
                History = "امیدبانک در سال ۱۴۰۳ توسط بانک قرض‌الحسنه مهر ایران راه‌اندازی شد. این ترابانک با هدف دسترسی آسان‌تر به خدمات قرض‌الحسنه از طریق گوشی همراه ایجاد شده و امکان ثبت درخواست وام، پیگیری تسهیلات و مدیریت حساب را فراهم می‌کند."
            },
            // ── 29. باجت ──
            new Bank
            {
                Id = 29, Name = "باجت", Slug = "bajet", LogoUrl = logo + "tejarat-300-wc.png",
                Website = "https://bajet.ir", DisplayOrder = 29, IsActive = true, CreatedAt = d,
                BankType = "neobank", ParentBankId = 4, OwnershipType = "ترابانک",
                FoundedYear = 1402, HeadquartersCity = "تهران",
                Description = "باجت ترابانک بانک تجارت با تمرکز بر مدیریت مالی شخصی و بودجه‌بندی هوشمند است.",
                History = "باجت در سال ۱۴۰۲ توسط بانک تجارت راه‌اندازی شد. نام این ترابانک از واژه Budget (بودجه) الهام گرفته شده و بر ابزارهای مدیریت مالی شخصی، بودجه‌بندی و کنترل هزینه‌ها تمرکز دارد."
            },
            // ── 30. باران ──
            new Bank
            {
                Id = 30, Name = "باران", Slug = "baran", LogoUrl = logo + "bki-300-wc.png",
                Website = "https://baran.bank", DisplayOrder = 30, IsActive = true, CreatedAt = d,
                BankType = "neobank", ParentBankId = 9, OwnershipType = "ترابانک",
                FoundedYear = 1399, HeadquartersCity = "تهران",
                Description = "باران ترابانک بانک کشاورزی با خدمات بانکداری دیجیتال ویژه مناطق روستایی و کشاورزان است.",
                History = "باران در سال ۱۳۹۹ توسط بانک کشاورزی راه‌اندازی شد. این ترابانک با هدف ارائه خدمات بانکداری دیجیتال به کشاورزان و ساکنین مناطق روستایی ایجاد شده و دسترسی آسان به خدمات بانکی را از طریق تلفن همراه فراهم می‌کند."
            },
            // ── 31. بانکت ──
            new Bank
            {
                Id = 31, Name = "بانکت", Slug = "banket", LogoUrl = logo + "mehriran-300-wc.png",
                Website = "https://banket.ir", DisplayOrder = 31, IsActive = true, CreatedAt = d,
                BankType = "neobank", ParentBankId = 19, OwnershipType = "ترابانک",
                FoundedYear = 1398, HeadquartersCity = "تهران",
                Description = "بانکت نخستین ترابانک بانک قرض‌الحسنه مهر ایران و از قدیمی‌ترین نئوبانک‌های کشور است.",
                History = "بانکت در سال ۱۳۹۸ توسط بانک قرض‌الحسنه مهر ایران راه‌اندازی شد و یکی از قدیمی‌ترین ترابانک‌های ایران محسوب می‌شود. بانکت خدمات افتتاح حساب دیجیتال، انتقال وجه، پرداخت قبوض و درخواست تسهیلات قرض‌الحسنه را ارائه می‌دهد."
            },
            // ── 32. بانکواره ──
            new Bank
            {
                Id = 32, Name = "بانکواره", Slug = "bankoareh", LogoUrl = logo + "bki-300-wc.png",
                Website = "https://bankvareh.ir", DisplayOrder = 32, IsActive = true, CreatedAt = d,
                BankType = "neobank", ParentBankId = 9, OwnershipType = "ترابانک",
                FoundedYear = 1402, HeadquartersCity = "تهران",
                Description = "بانکواره دومین ترابانک بانک کشاورزی با خدمات دیجیتال مکمل اپلیکیشن باران است.",
                History = "بانکواره در سال ۱۴۰۲ توسط بانک کشاورزی راه‌اندازی شد. این ترابانک خدمات بانکداری دیجیتال مکمل و نوآورانه ارائه می‌دهد و بر تجربه کاربری ساده و دسترسی آسان تمرکز دارد."
            },
            // ── 33. سیبانک ──
            new Bank
            {
                Id = 33, Name = "سیبانک", Slug = "sibank", LogoUrl = logo + "sina-300-wc.png",
                Website = "https://sibank.ir", DisplayOrder = 33, IsActive = true, CreatedAt = d,
                BankType = "neobank", ParentBankId = 15, OwnershipType = "ترابانک",
                FoundedYear = 1402, HeadquartersCity = "تهران",
                Description = "سیبانک ترابانک بانک سینا با خدمات بانکداری موبایلی و کارت دیجیتال است.",
                History = "سیبانک در سال ۱۴۰۲ توسط بانک سینا راه‌اندازی شد. این ترابانک خدمات افتتاح حساب آنلاین، کارت بانکی دیجیتال، انتقال وجه و مدیریت مالی را از طریق اپلیکیشن موبایل ارائه می‌دهد."
            },
            // ── 34. فرارفاه ──
            new Bank
            {
                Id = 34, Name = "فرارفاه", Slug = "fararefah", LogoUrl = logo + "rb-300-wc.png",
                Website = "https://fararefah.ir", DisplayOrder = 34, IsActive = true, CreatedAt = d,
                BankType = "neobank", ParentBankId = 10, OwnershipType = "ترابانک",
                FoundedYear = 1402, HeadquartersCity = "تهران",
                Description = "فرارفاه ترابانک بانک رفاه کارگران با خدمات دیجیتال ویژه حقوق‌بگیران و بازنشستگان است.",
                History = "فرارفاه در سال ۱۴۰۲ توسط بانک رفاه کارگران راه‌اندازی شد. این ترابانک خدمات بانکداری دیجیتال را با تمرکز ویژه بر نیازهای حقوق‌بگیران، بازنشستگان و مشمولان تأمین اجتماعی ارائه می‌دهد."
            }
        );

        // ══════════════════════════════════════════════════════════════════
        //  LOAN TYPES
        // ══════════════════════════════════════════════════════════════════
        modelBuilder.Entity<LoanType>().HasData(
            new LoanType { Id = 1, Name = "وام مسکن", Slug = "vam-maskan", IconClass = "bi-house-door", DisplayOrder = 1, IsActive = true, Description = "انواع وام‌های خرید، ساخت و ودیعه مسکن" },
            new LoanType { Id = 2, Name = "وام ازدواج", Slug = "vam-ezdevaj", IconClass = "bi-heart", DisplayOrder = 2, IsActive = true, Description = "تسهیلات قرض‌الحسنه ازدواج برای زوجین" },
            new LoanType { Id = 3, Name = "وام خودرو", Slug = "vam-khodro", IconClass = "bi-car-front", DisplayOrder = 3, IsActive = true, Description = "وام خرید خودرو داخلی و خارجی" },
            new LoanType { Id = 4, Name = "وام تحصیلی", Slug = "vam-tahsili", IconClass = "bi-mortarboard", DisplayOrder = 4, IsActive = true, Description = "وام‌های دانشجویی و تحصیلی" },
            new LoanType { Id = 5, Name = "وام کسب و کار", Slug = "vam-kasb-kar", IconClass = "bi-briefcase", DisplayOrder = 5, IsActive = true, Description = "تسهیلات اشتغال‌زایی، سرمایه در گردش و کارآفرینی" },
            new LoanType { Id = 6, Name = "وام خرید کالا", Slug = "vam-kala", IconClass = "bi-cart-check", DisplayOrder = 6, IsActive = true, Description = "وام خرید لوازم خانگی، الکترونیک و کالاهای اساسی" },
            new LoanType { Id = 7, Name = "تسهیلات قرض‌الحسنه", Slug = "gharz-ol-hasaneh", IconClass = "bi-gift", DisplayOrder = 7, IsActive = true, Description = "تسهیلات بدون سود یا کم‌بهره قرض‌الحسنه" },
            new LoanType { Id = 8, Name = "وام آنلاین", Slug = "vam-online", IconClass = "bi-phone", DisplayOrder = 8, IsActive = true, Description = "وام‌های آنلاین بدون نیاز به مراجعه حضوری" },
            new LoanType { Id = 9, Name = "وام بدون ضامن", Slug = "vam-bedone-zamen", IconClass = "bi-person-check", DisplayOrder = 9, IsActive = true, Description = "تسهیلاتی که بدون نیاز به ضامن قابل دریافت هستند" },
            new LoanType { Id = 10, Name = "وام فوری", Slug = "vam-fori", IconClass = "bi-lightning", DisplayOrder = 10, IsActive = true, Description = "وام‌های سریع و فوری با فرآیند ساده" },
            new LoanType { Id = 11, Name = "تسهیلات جعاله", Slug = "tashilat-joaleh", IconClass = "bi-tools", DisplayOrder = 11, IsActive = true, Description = "تسهیلات جعاله برای تعمیرات و خدمات" },
            new LoanType { Id = 12, Name = "وام ودیعه مسکن", Slug = "vam-odieh-maskan", IconClass = "bi-key", DisplayOrder = 12, IsActive = true, Description = "وام‌های پرداخت ودیعه اجاره مسکن" }
        );

        // ══════════════════════════════════════════════════════════════════
        //  LOANS
        // ══════════════════════════════════════════════════════════════════
        modelBuilder.Entity<Loan>().HasData(

            // ────────────────────── بانک ملی (1) ──────────────────────
            new Loan
            {
                Id = 1, Title = "وام ازدواج بانک ملی", Slug = "vam-ezdevaj-bank-melli", BankId = 1, LoanTypeId = 2,
                ShortDescription = "وام قرض‌الحسنه ازدواج ۳۰۰ میلیون تومانی با نرخ ۴ درصد",
                FullDescription = "وام ازدواج بانک ملی ایران از محل منابع قرض‌الحسنه به زوجین جوان ارائه می‌شود. مبلغ این وام برای هر یک از زوجین ۳۰۰ میلیون تومان و برای زوج‌هایی که داماد زیر ۲۵ سال و عروس زیر ۲۳ سال باشد، ۳۵۰ میلیون تومان است.\n\nشرایط:\n• تابعیت ایرانی\n• عقدنامه رسمی پس از ابتدای سال ۱۴۰۰\n• عدم دریافت وام ازدواج قبلی\n• اعتبارسنجی مثبت\n\nمدارک:\n• شناسنامه و کارت ملی زوجین\n• عقدنامه\n• گواهی اشتغال یا فیش حقوقی\n• ضامن معتبر",
                InterestRate = 4, MinAmount = 300_000_000, MaxAmount = 350_000_000, RepaymentMonths = 120,
                Requirements = "عقدنامه رسمی، شناسنامه و کارت ملی زوجین، ضامن معتبر، اعتبارسنجی مثبت",
                IsActive = true, IsFeatured = true, ViewCount = 4250, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.bmi.ir/fa/facility-services"
            },
            new Loan
            {
                Id = 2, Title = "وام مسکن بانک ملی", Slug = "vam-maskan-bank-melli", BankId = 1, LoanTypeId = 1,
                ShortDescription = "تسهیلات خرید مسکن تا ۸۰۰ میلیون تومان با سود ۱۸ درصد",
                FullDescription = "بانک ملی ایران تسهیلات خرید مسکن را از محل منابع بانک ارائه می‌دهد.\n\nویژگی‌ها:\n• سقف تسهیلات: ۸۰۰ میلیون تومان\n• نرخ سود: ۱۸ درصد\n• بازپرداخت: ۱۴۴ ماه (۱۲ سال)\n• وثیقه: سند ملک خریداری‌شده\n\nمدارک:\n• مبایعه‌نامه یا قولنامه\n• ارزیابی ملک توسط کارشناس بانک\n• فیش حقوقی یا جواز کسب\n• ضامن معتبر",
                InterestRate = 18, MinAmount = 200_000_000, MaxAmount = 800_000_000, RepaymentMonths = 144,
                Requirements = "مبایعه‌نامه، ارزیابی ملک، فیش حقوقی، ضامن",
                IsActive = true, IsFeatured = true, ViewCount = 3800, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.bmi.ir/fa/facility-services"
            },
            new Loan
            {
                Id = 3, Title = "وام خودرو بانک ملی", Slug = "vam-khodro-bank-melli", BankId = 1, LoanTypeId = 3,
                ShortDescription = "وام خرید خودرو داخلی تا ۵۰۰ میلیون تومان",
                FullDescription = "بانک ملی ایران وام خرید خودرو داخلی و مونتاژی را ارائه می‌دهد.\n\nشرایط:\n• سقف: ۵۰۰ میلیون تومان\n• نرخ سود: ۲۳ درصد (مرابحه)\n• بازپرداخت: ۶۰ ماه\n• خودرو به عنوان وثیقه",
                InterestRate = 23, MaxAmount = 500_000_000, RepaymentMonths = 60,
                Requirements = "پیش‌فاکتور خودرو، فیش حقوقی، ضامن",
                IsActive = true, ViewCount = 2100, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.bmi.ir/fa/facility-services"
            },

            // ────────────────────── بانک ملت (2) ──────────────────────
            new Loan
            {
                Id = 4, Title = "وام ازدواج بانک ملت", Slug = "vam-ezdevaj-bank-mellat", BankId = 2, LoanTypeId = 2,
                ShortDescription = "وام ازدواج ۳۰۰ میلیون تومانی با کارمزد ۴ درصد",
                FullDescription = "بانک ملت وام ازدواج قرض‌الحسنه را مطابق مصوبات بانک مرکزی ارائه می‌دهد.\n\nمبلغ: ۳۰۰ تا ۳۵۰ میلیون تومان\nکارمزد: ۴ درصد\nبازپرداخت: ۱۲۰ ماه\n\nمزایا:\n• ثبت‌نام آنلاین از طریق سامانه بام\n• اعتبارسنجی سریع\n• امکان انتخاب شعبه دلخواه",
                InterestRate = 4, MinAmount = 300_000_000, MaxAmount = 350_000_000, RepaymentMonths = 120,
                Requirements = "عقدنامه رسمی، مدارک شناسایی، ضامن",
                IsActive = true, IsFeatured = true, ViewCount = 3900, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.bankmellat.ir/facilities"
            },
            new Loan
            {
                Id = 5, Title = "وام خودرو بانک ملت", Slug = "vam-khodro-bank-mellat", BankId = 2, LoanTypeId = 3,
                ShortDescription = "وام خرید خودرو تا ۴۰۰ میلیون تومان با بازپرداخت ۶۰ ماهه",
                FullDescription = "بانک ملت تسهیلات خرید خودرو داخلی و مونتاژ را ارائه می‌دهد.\n\n• سقف: ۴۰۰ میلیون تومان\n• سود: ۲۳ درصد\n• بازپرداخت: ۶۰ ماه\n• خودرو به رهن بانک",
                InterestRate = 23, MaxAmount = 400_000_000, RepaymentMonths = 60,
                Requirements = "پیش‌فاکتور خودرو، اعتبارسنجی، ضامن",
                IsActive = true, ViewCount = 1800, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.bankmellat.ir/facilities"
            },
            new Loan
            {
                Id = 6, Title = "وام کسب و کار بانک ملت", Slug = "vam-kasb-kar-bank-mellat", BankId = 2, LoanTypeId = 5,
                ShortDescription = "تسهیلات سرمایه در گردش تا ۲ میلیارد تومان",
                FullDescription = "بانک ملت تسهیلات سرمایه در گردش برای بنگاه‌های اقتصادی ارائه می‌دهد.\n\n• سقف: ۲ میلیارد تومان\n• سود: ۲۳ درصد\n• بازپرداخت: ۳۶ ماه\n• وثیقه: سند ملکی یا ضمانت‌نامه بانکی",
                InterestRate = 23, MaxAmount = 2_000_000_000, RepaymentMonths = 36,
                Requirements = "جواز کسب، صورت‌های مالی، وثیقه ملکی",
                IsActive = true, ViewCount = 1200, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.bankmellat.ir/facilities"
            },

            // ────────────────────── بانک صادرات (3) ──────────────────────
            new Loan
            {
                Id = 7, Title = "وام ازدواج بانک صادرات", Slug = "vam-ezdevaj-bank-saderat", BankId = 3, LoanTypeId = 2,
                ShortDescription = "وام ازدواج ۳۰۰ میلیون تومانی با بازپرداخت ۱۰ ساله",
                FullDescription = "بانک صادرات ایران وام ازدواج قرض‌الحسنه را طبق شرایط بانک مرکزی ارائه می‌دهد.\n\nمبلغ: ۳۰۰ تا ۳۵۰ میلیون تومان\nکارمزد: ۴ درصد\nبازپرداخت: ۱۲۰ ماه",
                InterestRate = 4, MinAmount = 300_000_000, MaxAmount = 350_000_000, RepaymentMonths = 120,
                Requirements = "عقدنامه رسمی، مدارک شناسایی، ضامن",
                IsActive = true, ViewCount = 3200, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.bsi.ir/fa/page/1/facilities"
            },
            new Loan
            {
                Id = 8, Title = "وام خرید کالا بانک صادرات", Slug = "vam-kala-bank-saderat", BankId = 3, LoanTypeId = 6,
                ShortDescription = "وام خرید لوازم خانگی و الکترونیک تا ۱۵۰ میلیون تومان",
                FullDescription = "بانک صادرات تسهیلات خرید کالا از فروشگاه‌های طرف قرارداد ارائه می‌دهد.\n\n• سقف: ۱۵۰ میلیون تومان\n• سود: ۲۳ درصد\n• بازپرداخت: ۲۴ ماه\n• خرید از فروشگاه‌های مجاز",
                InterestRate = 23, MaxAmount = 150_000_000, RepaymentMonths = 24,
                Requirements = "فاکتور فروشگاه طرف قرارداد، فیش حقوقی",
                IsActive = true, ViewCount = 890, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.bsi.ir/fa/page/1/facilities"
            },
            new Loan
            {
                Id = 9, Title = "وام مسکن بانک صادرات", Slug = "vam-maskan-bank-saderat", BankId = 3, LoanTypeId = 1,
                ShortDescription = "تسهیلات خرید مسکن تا ۶۰۰ میلیون تومان",
                FullDescription = "بانک صادرات ایران تسهیلات خرید مسکن ارائه می‌دهد.\n\n• سقف: ۶۰۰ میلیون تومان\n• سود: ۱۸ درصد\n• بازپرداخت: ۱۲۰ ماه\n• وثیقه: سند ملک",
                InterestRate = 18, MaxAmount = 600_000_000, RepaymentMonths = 120,
                Requirements = "مبایعه‌نامه، ارزیابی ملک، ضامن",
                IsActive = true, ViewCount = 2600, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.bsi.ir/fa/page/1/facilities"
            },

            // ────────────────────── بانک تجارت (4) ──────────────────────
            new Loan
            {
                Id = 10, Title = "وام ازدواج بانک تجارت", Slug = "vam-ezdevaj-bank-tejarat", BankId = 4, LoanTypeId = 2,
                ShortDescription = "وام ازدواج ۳۰۰ میلیون تومانی بانک تجارت",
                FullDescription = "بانک تجارت وام ازدواج قرض‌الحسنه ارائه می‌دهد.\n\nمبلغ: ۳۰۰ تا ۳۵۰ میلیون تومان\nکارمزد: ۴ درصد\nبازپرداخت: ۱۲۰ ماه",
                InterestRate = 4, MinAmount = 300_000_000, MaxAmount = 350_000_000, RepaymentMonths = 120,
                Requirements = "عقدنامه رسمی، مدارک شناسایی، ضامن",
                IsActive = true, ViewCount = 2800, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.tejaratbank.ir/web_directory/1-facilities.html"
            },
            new Loan
            {
                Id = 11, Title = "وام کسب و کار بانک تجارت", Slug = "vam-kasb-kar-bank-tejarat", BankId = 4, LoanTypeId = 5,
                ShortDescription = "تسهیلات سرمایه در گردش تا ۳ میلیارد تومان",
                FullDescription = "بانک تجارت تسهیلات سرمایه در گردش و تجاری ارائه می‌دهد.\n\n• سقف: ۳ میلیارد تومان\n• سود: ۲۳ درصد\n• بازپرداخت: ۲۴ تا ۳۶ ماه\n• وثیقه: سند ملکی، ضمانت‌نامه بانکی یا اوراق بهادار",
                InterestRate = 23, MaxAmount = 3_000_000_000, RepaymentMonths = 36,
                Requirements = "جواز کسب، صورت‌های مالی، وثیقه",
                IsActive = true, IsFeatured = true, ViewCount = 1500, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.tejaratbank.ir/web_directory/1-facilities.html"
            },

            // ────────────────────── بانک سپه (5) ──────────────────────
            new Loan
            {
                Id = 12, Title = "وام ازدواج بانک سپه", Slug = "vam-ezdevaj-bank-sepah", BankId = 5, LoanTypeId = 2,
                ShortDescription = "وام ازدواج ۳۰۰ میلیون تومانی بانک سپه",
                FullDescription = "بانک سپه وام ازدواج قرض‌الحسنه ارائه می‌دهد.\n\nمبلغ: ۳۰۰ تا ۳۵۰ میلیون تومان\nکارمزد: ۴ درصد\nبازپرداخت: ۱۲۰ ماه\n\nبا توجه به ادغام بانک‌های متعدد در بانک سپه، شبکه گسترده‌ای برای ثبت‌نام وام ازدواج فراهم است.",
                InterestRate = 4, MinAmount = 300_000_000, MaxAmount = 350_000_000, RepaymentMonths = 120,
                Requirements = "عقدنامه رسمی، مدارک شناسایی، ضامن",
                IsActive = true, ViewCount = 3100, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.banksepah.ir/fa/facilities"
            },
            new Loan
            {
                Id = 13, Title = "وام خودرو بانک سپه", Slug = "vam-khodro-bank-sepah", BankId = 5, LoanTypeId = 3,
                ShortDescription = "وام خرید خودرو تا ۴۰۰ میلیون تومان",
                FullDescription = "بانک سپه تسهیلات خرید خودرو داخلی ارائه می‌دهد.\n\n• سقف: ۴۰۰ میلیون تومان\n• سود: ۲۳ درصد\n• بازپرداخت: ۶۰ ماه",
                InterestRate = 23, MaxAmount = 400_000_000, RepaymentMonths = 60,
                Requirements = "پیش‌فاکتور خودرو، فیش حقوقی، ضامن",
                IsActive = true, ViewCount = 1400, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.banksepah.ir/fa/facilities"
            },
            new Loan
            {
                Id = 14, Title = "قرض‌الحسنه بانک سپه", Slug = "gharz-hasaneh-bank-sepah", BankId = 5, LoanTypeId = 7,
                ShortDescription = "قرض‌الحسنه تا ۱۵۰ میلیون تومان با کارمزد ۴ درصد",
                FullDescription = "بانک سپه تسهیلات قرض‌الحسنه برای رفع نیازهای ضروری ارائه می‌دهد.\n\n• سقف: ۱۵۰ میلیون تومان\n• کارمزد: ۴ درصد\n• بازپرداخت: ۳۶ ماه",
                InterestRate = 4, MaxAmount = 150_000_000, RepaymentMonths = 36,
                Requirements = "سپرده‌گذاری، ضامن معتبر",
                IsActive = true, ViewCount = 900, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.banksepah.ir/fa/facilities"
            },

            // ────────────────────── بانک پاسارگاد (6) ──────────────────────
            new Loan
            {
                Id = 15, Title = "وام ازدواج بانک پاسارگاد", Slug = "vam-ezdevaj-bank-pasargad", BankId = 6, LoanTypeId = 2,
                ShortDescription = "وام ازدواج ۳۰۰ میلیون تومانی با فرآیند سریع",
                FullDescription = "بانک پاسارگاد وام ازدواج قرض‌الحسنه ارائه می‌دهد.\n\nمبلغ: ۳۰۰ تا ۳۵۰ میلیون تومان\nکارمزد: ۴ درصد\nبازپرداخت: ۱۲۰ ماه\n\nمزایا:\n• ثبت‌نام آنلاین\n• فرآیند اعتبارسنجی سریع\n• پشتیبانی ۲۴ ساعته",
                InterestRate = 4, MinAmount = 300_000_000, MaxAmount = 350_000_000, RepaymentMonths = 120,
                Requirements = "عقدنامه رسمی، مدارک شناسایی، ضامن",
                IsActive = true, IsFeatured = true, ViewCount = 5200, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.bpi.ir/fa/personal-banking/facilities"
            },
            new Loan
            {
                Id = 16, Title = "وام خودرو بانک پاسارگاد", Slug = "vam-khodro-bank-pasargad", BankId = 6, LoanTypeId = 3,
                ShortDescription = "وام خرید خودرو تا ۶۰۰ میلیون تومان با نرخ رقابتی",
                FullDescription = "بانک پاسارگاد تسهیلات خرید خودرو ارائه می‌دهد.\n\n• سقف: ۶۰۰ میلیون تومان\n• سود: ۲۱ درصد (رقابتی‌تر)\n• بازپرداخت: ۶۰ ماه\n• خودرو به رهن بانک\n\nمزایا:\n• امکان خرید خودرو داخلی و مونتاژ\n• اعتبارسنجی سریع",
                InterestRate = 21, MaxAmount = 600_000_000, RepaymentMonths = 60,
                Requirements = "پیش‌فاکتور خودرو، فیش حقوقی، بیمه‌نامه",
                IsActive = true, IsFeatured = true, ViewCount = 4100, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.bpi.ir/fa/personal-banking/facilities"
            },
            new Loan
            {
                Id = 17, Title = "وام کسب و کار بانک پاسارگاد", Slug = "vam-kasb-kar-pasargad", BankId = 6, LoanTypeId = 5,
                ShortDescription = "تسهیلات سرمایه در گردش تا ۵ میلیارد تومان",
                FullDescription = "بانک پاسارگاد تسهیلات سرمایه در گردش و تأمین مالی بنگاه‌های اقتصادی ارائه می‌دهد.\n\n• سقف: ۵ میلیارد تومان\n• سود: ۲۳ درصد\n• بازپرداخت: ۱۲ تا ۳۶ ماه\n• وثیقه: سند ملکی، سپرده یا ضمانت‌نامه",
                InterestRate = 23, MaxAmount = 5_000_000_000, RepaymentMonths = 36,
                Requirements = "جواز کسب، صورت‌های مالی، وثیقه",
                IsActive = true, ViewCount = 2700, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.bpi.ir/fa/personal-banking/facilities"
            },

            // ────────────────────── بانک پارسیان (7) ──────────────────────
            new Loan
            {
                Id = 18, Title = "وام ازدواج بانک پارسیان", Slug = "vam-ezdevaj-bank-parsian", BankId = 7, LoanTypeId = 2,
                ShortDescription = "وام ازدواج ۳۰۰ میلیون تومانی بانک پارسیان",
                FullDescription = "بانک پارسیان وام ازدواج قرض‌الحسنه ارائه می‌دهد.\n\nمبلغ: ۳۰۰ تا ۳۵۰ میلیون تومان\nکارمزد: ۴ درصد\nبازپرداخت: ۱۲۰ ماه",
                InterestRate = 4, MinAmount = 300_000_000, MaxAmount = 350_000_000, RepaymentMonths = 120,
                Requirements = "عقدنامه رسمی، مدارک شناسایی، ضامن",
                IsActive = true, ViewCount = 2400, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.parsian-bank.ir/facilities"
            },
            new Loan
            {
                Id = 19, Title = "وام خرید کالا بانک پارسیان", Slug = "vam-kala-bank-parsian", BankId = 7, LoanTypeId = 6,
                ShortDescription = "وام خرید لوازم خانگی تا ۲۰۰ میلیون تومان",
                FullDescription = "بانک پارسیان تسهیلات خرید کالا ارائه می‌دهد.\n\n• سقف: ۲۰۰ میلیون تومان\n• سود: ۲۳ درصد\n• بازپرداخت: ۲۴ ماه",
                InterestRate = 23, MaxAmount = 200_000_000, RepaymentMonths = 24,
                Requirements = "فاکتور فروشگاه طرف قرارداد، فیش حقوقی",
                IsActive = true, ViewCount = 750, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.parsian-bank.ir/facilities"
            },
            new Loan
            {
                Id = 20, Title = "وام مسکن بانک پارسیان", Slug = "vam-maskan-bank-parsian", BankId = 7, LoanTypeId = 1,
                ShortDescription = "تسهیلات خرید مسکن تا ۵۰۰ میلیون تومان",
                FullDescription = "بانک پارسیان تسهیلات خرید مسکن ارائه می‌دهد.\n\n• سقف: ۵۰۰ میلیون تومان\n• سود: ۱۸ درصد\n• بازپرداخت: ۱۲۰ ماه",
                InterestRate = 18, MaxAmount = 500_000_000, RepaymentMonths = 120,
                Requirements = "مبایعه‌نامه، ارزیابی ملک، ضامن",
                IsActive = true, ViewCount = 1900, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.parsian-bank.ir/facilities"
            },

            // ────────────────────── بانک مسکن (8) ──────────────────────
            new Loan
            {
                Id = 21, Title = "وام خرید مسکن بانک مسکن", Slug = "vam-kharid-maskan", BankId = 8, LoanTypeId = 1,
                ShortDescription = "وام خرید مسکن تا ۶۰۰ میلیون تومان از صندوق یکم",
                FullDescription = "بانک مسکن تسهیلات خرید مسکن از صندوق پس‌انداز یکم ارائه می‌دهد.\n\n• سقف تهران: ۶۰۰ میلیون تومان\n• سقف شهرهای بزرگ: ۴۰۰ میلیون تومان\n• سود: ۱۷.۵ درصد\n• بازپرداخت: ۱۴۴ ماه\n\nشرایط:\n• افتتاح حساب صندوق یکم حداقل ۶ ماه قبل\n• پس‌انداز ماهانه منظم\n• ملک در محدوده شهری",
                InterestRate = 17.5m, MinAmount = 200_000_000, MaxAmount = 600_000_000, RepaymentMonths = 144,
                Requirements = "حساب صندوق یکم، مبایعه‌نامه، ارزیابی ملک",
                IsActive = true, IsFeatured = true, ViewCount = 8500, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.bank-maskan.ir/fa/facilities"
            },
            new Loan
            {
                Id = 22, Title = "وام نوسازی و بهسازی مسکن", Slug = "vam-nosazi-maskan", BankId = 8, LoanTypeId = 1,
                ShortDescription = "تسهیلات نوسازی و تعمیر مسکن تا ۳۰۰ میلیون تومان",
                FullDescription = "بانک مسکن تسهیلات نوسازی و بهسازی واحدهای مسکونی ارائه می‌دهد.\n\n• سقف: ۳۰۰ میلیون تومان\n• سود: ۱۸ درصد\n• بازپرداخت: ۶۰ ماه\n• وثیقه: سند ملک",
                InterestRate = 18, MaxAmount = 300_000_000, RepaymentMonths = 60,
                Requirements = "سند مالکیت، نقشه نوسازی، ارزیابی ملک",
                IsActive = true, ViewCount = 2300, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.bank-maskan.ir/fa/facilities"
            },
            new Loan
            {
                Id = 23, Title = "وام ساخت مسکن بانک مسکن", Slug = "vam-sakht-maskan", BankId = 8, LoanTypeId = 1,
                ShortDescription = "تسهیلات ساخت مسکن تا ۶۰۰ میلیون تومان",
                FullDescription = "بانک مسکن تسهیلات ساخت مسکن ارائه می‌دهد.\n\n• سقف: ۶۰۰ میلیون تومان\n• سود: ۱۸ درصد\n• بازپرداخت: ۱۸۰ ماه\n• وثیقه: سند زمین\n• نظارت مهندس ناظر\n• بیمه ساختمان",
                InterestRate = 18, MinAmount = 200_000_000, MaxAmount = 600_000_000, RepaymentMonths = 180,
                Requirements = "پروانه ساخت، سند زمین، نقشه مصوب، بیمه ساختمان",
                IsActive = true, ViewCount = 2300, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.bank-maskan.ir/fa/facilities"
            },
            new Loan
            {
                Id = 24, Title = "وام ازدواج بانک مسکن", Slug = "vam-ezdevaj-bank-maskan", BankId = 8, LoanTypeId = 2,
                ShortDescription = "وام ازدواج ۳۰۰ میلیون تومانی بانک مسکن",
                FullDescription = "بانک مسکن وام ازدواج قرض‌الحسنه ارائه می‌دهد.\n\nمبلغ: ۳۰۰ تا ۳۵۰ میلیون تومان\nکارمزد: ۴ درصد\nبازپرداخت: ۱۲۰ ماه",
                InterestRate = 4, MinAmount = 300_000_000, MaxAmount = 350_000_000, RepaymentMonths = 120,
                Requirements = "عقدنامه رسمی، مدارک شناسایی، ضامن",
                IsActive = true, ViewCount = 1500, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.bank-maskan.ir/fa/facilities"
            },
            new Loan
            {
                Id = 25, Title = "وام ودیعه مسکن بانک مسکن", Slug = "vam-odieh-maskan", BankId = 8, LoanTypeId = 1,
                ShortDescription = "وام ودیعه مسکن مستأجران تا ۲۷۵ میلیون تومان",
                FullDescription = "وام ودیعه مسکن برای کمک به مستأجران جهت پرداخت ودیعه اجاره ارائه می‌شود.\n\nسقف بر اساس شهر:\n• تهران: ۲۷۵ میلیون تومان\n• کلانشهرها: ۲۱۰ میلیون تومان\n• شهرهای بالای ۲۰۰ هزار نفر: ۱۴۰ میلیون تومان\n• سایر شهرها: ۱۰۰ میلیون تومان\n• روستاها: ۵۵ میلیون تومان",
                InterestRate = 23, MinAmount = 55_000_000, MaxAmount = 275_000_000, RepaymentMonths = 36,
                Requirements = "اجاره‌نامه رسمی، مدارک شناسایی، ضامن یا سفته",
                IsActive = true, IsFeatured = true, ViewCount = 9200, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.bank-maskan.ir/fa/facilities"
            },

            // ────────────────────── بانک کشاورزی (9) ──────────────────────
            new Loan
            {
                Id = 26, Title = "وام ازدواج بانک کشاورزی", Slug = "vam-ezdevaj-bank-keshavarzi", BankId = 9, LoanTypeId = 2,
                ShortDescription = "وام ازدواج ۳۰۰ میلیون تومانی بانک کشاورزی",
                FullDescription = "بانک کشاورزی وام ازدواج قرض‌الحسنه ارائه می‌دهد. با شبکه گسترده شعب در مناطق روستایی، دسترسی آسان برای ساکنین مناطق کم‌برخوردار فراهم است.",
                InterestRate = 4, MinAmount = 300_000_000, MaxAmount = 350_000_000, RepaymentMonths = 120,
                Requirements = "عقدنامه رسمی، مدارک شناسایی، ضامن",
                IsActive = true, ViewCount = 1800, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.bki.ir/fa/facilities"
            },
            new Loan
            {
                Id = 27, Title = "تسهیلات کشاورزی و دامداری", Slug = "vam-keshavarzi-damdari", BankId = 9, LoanTypeId = 5,
                ShortDescription = "تسهیلات تولیدی کشاورزی تا ۵ میلیارد تومان",
                FullDescription = "بانک کشاورزی تسهیلات ویژه بخش‌های کشاورزی، دامداری، باغداری و صنایع تبدیلی ارائه می‌دهد.\n\nحوزه‌ها:\n• زراعت و باغبانی\n• دامداری و مرغداری\n• شیلات و آبزی‌پروری\n• صنایع غذایی و تبدیلی\n• گلخانه‌داری\n• ماشین‌آلات کشاورزی",
                InterestRate = 18, MinAmount = 100_000_000, MaxAmount = 5_000_000_000, RepaymentMonths = 60,
                Requirements = "مجوز فعالیت کشاورزی، طرح توجیهی، وثیقه ملکی",
                IsActive = true, IsFeatured = true, ViewCount = 3400, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.bki.ir/fa/facilities"
            },
            new Loan
            {
                Id = 28, Title = "قرض‌الحسنه بانک کشاورزی", Slug = "gharz-hasaneh-bank-keshavarzi", BankId = 9, LoanTypeId = 7,
                ShortDescription = "قرض‌الحسنه تا ۱۰۰ میلیون تومان با کارمزد ۴ درصد",
                FullDescription = "بانک کشاورزی تسهیلات قرض‌الحسنه را به مشتریان خوش‌حساب ارائه می‌دهد.\n\n• سقف: ۱۰۰ میلیون تومان\n• کارمزد: ۴ درصد\n• بازپرداخت: ۳۶ ماه",
                InterestRate = 4, MaxAmount = 100_000_000, RepaymentMonths = 36,
                Requirements = "حساب قرض‌الحسنه فعال، سپرده‌گذاری، ضامن",
                IsActive = true, ViewCount = 600, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.bki.ir/fa/facilities"
            },

            // ────────────────────── بانک رفاه (10) ──────────────────────
            new Loan
            {
                Id = 29, Title = "وام ازدواج بانک رفاه", Slug = "vam-ezdevaj-bank-refah", BankId = 10, LoanTypeId = 2,
                ShortDescription = "وام ازدواج ۳۰۰ میلیون تومانی بانک رفاه کارگران",
                FullDescription = "بانک رفاه کارگران وام ازدواج قرض‌الحسنه ارائه می‌دهد.\n\nمزایا:\n• اولویت‌بندی برای بیمه‌شدگان تأمین اجتماعی\n• فرآیند تسهیل‌شده\n• شعبات گسترده",
                InterestRate = 4, MinAmount = 300_000_000, MaxAmount = 350_000_000, RepaymentMonths = 120,
                Requirements = "عقدنامه رسمی، مدارک شناسایی، ضامن",
                IsActive = true, ViewCount = 1600, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.refah-bank.ir/facilities"
            },
            new Loan
            {
                Id = 30, Title = "وام خرید کالا بانک رفاه", Slug = "vam-kala-bank-refah", BankId = 10, LoanTypeId = 6,
                ShortDescription = "وام خرید کالا و لوازم خانگی تا ۲۰۰ میلیون تومان",
                FullDescription = "بانک رفاه تسهیلات خرید کالا از فروشگاه‌های طرف قرارداد ارائه می‌دهد.\n\n• سقف: ۲۰۰ میلیون تومان\n• سود: ۲۳ درصد\n• بازپرداخت: ۳۶ ماه",
                InterestRate = 23, MaxAmount = 200_000_000, RepaymentMonths = 36,
                Requirements = "فاکتور فروشگاه طرف قرارداد، فیش حقوقی، اعتبارسنجی",
                IsActive = true, ViewCount = 520, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.refah-bank.ir/facilities"
            },

            // ────────────────────── پست بانک (11) ──────────────────────
            new Loan
            {
                Id = 31, Title = "وام ازدواج پست بانک", Slug = "vam-ezdevaj-post-bank", BankId = 11, LoanTypeId = 2,
                ShortDescription = "وام ازدواج ۳۰۰ میلیون تومانی پست بانک ایران",
                FullDescription = "پست بانک ایران وام ازدواج قرض‌الحسنه ارائه می‌دهد.\n\nمبلغ: ۳۰۰ تا ۳۵۰ میلیون تومان\nکارمزد: ۴ درصد\nبازپرداخت: ۱۲۰ ماه\n\nمزایا:\n• دسترسی گسترده از طریق دفاتر پستی\n• خدمات‌رسانی در مناطق روستایی و محروم",
                InterestRate = 4, MinAmount = 300_000_000, MaxAmount = 350_000_000, RepaymentMonths = 120,
                Requirements = "عقدنامه رسمی، مدارک شناسایی، ضامن",
                IsActive = true, ViewCount = 980, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.postbank.ir/fa/facilities"
            },
            new Loan
            {
                Id = 32, Title = "تسهیلات روستایی پست بانک", Slug = "vam-rustaei-post-bank", BankId = 11, LoanTypeId = 5,
                ShortDescription = "تسهیلات اشتغال‌زایی روستایی تا ۵۰۰ میلیون تومان",
                FullDescription = "پست بانک ایران تسهیلات ویژه اشتغال‌زایی در مناطق روستایی ارائه می‌دهد.\n\n• سقف: ۵۰۰ میلیون تومان\n• سود: ۱۸ درصد\n• بازپرداخت: ۴۸ ماه\n\nحوزه‌ها:\n• مشاغل خانگی\n• صنایع دستی\n• کشاورزی کوچک‌مقیاس\n• خدمات روستایی",
                InterestRate = 18, MaxAmount = 500_000_000, RepaymentMonths = 48,
                Requirements = "طرح توجیهی، مجوز فعالیت، سکونت در روستا",
                IsActive = true, IsFeatured = true, ViewCount = 1350, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.postbank.ir/fa/facilities"
            },
            new Loan
            {
                Id = 33, Title = "قرض‌الحسنه پست بانک", Slug = "gharz-hasaneh-post-bank", BankId = 11, LoanTypeId = 7,
                ShortDescription = "قرض‌الحسنه تا ۸۰ میلیون تومان",
                FullDescription = "پست بانک ایران تسهیلات قرض‌الحسنه ارائه می‌دهد.\n\n• سقف: ۸۰ میلیون تومان\n• کارمزد: ۴ درصد\n• بازپرداخت: ۳۶ ماه",
                InterestRate = 4, MaxAmount = 80_000_000, RepaymentMonths = 36,
                Requirements = "حساب فعال در پست بانک، ضامن",
                IsActive = true, ViewCount = 450, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.postbank.ir/fa/facilities"
            },

            // ────────────────────── بانک سامان (12) ──────────────────────
            new Loan
            {
                Id = 34, Title = "وام ازدواج بانک سامان", Slug = "vam-ezdevaj-bank-saman", BankId = 12, LoanTypeId = 2,
                ShortDescription = "وام ازدواج ۳۰۰ میلیون تومانی بانک سامان",
                FullDescription = "بانک سامان وام ازدواج قرض‌الحسنه ارائه می‌دهد.\n\nمبلغ: ۳۰۰ تا ۳۵۰ میلیون تومان\nکارمزد: ۴ درصد\nبازپرداخت: ۱۲۰ ماه\n\nمزایا:\n• ثبت‌نام آنلاین از سامانه اینترنتی\n• فرآیند سریع تأیید\n• پیگیری آنلاین وضعیت درخواست",
                InterestRate = 4, MinAmount = 300_000_000, MaxAmount = 350_000_000, RepaymentMonths = 120,
                Requirements = "عقدنامه رسمی، مدارک شناسایی، ضامن",
                IsActive = true, ViewCount = 2100, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.sb24.ir/personal/facilities"
            },
            new Loan
            {
                Id = 35, Title = "وام خودرو بانک سامان", Slug = "vam-khodro-bank-saman", BankId = 12, LoanTypeId = 3,
                ShortDescription = "وام خرید خودرو تا ۵۰۰ میلیون تومان",
                FullDescription = "بانک سامان تسهیلات خرید خودرو داخلی و مونتاژ ارائه می‌دهد.\n\n• سقف: ۵۰۰ میلیون تومان\n• سود: ۲۳ درصد\n• بازپرداخت: ۶۰ ماه\n• خودرو به رهن بانک",
                InterestRate = 23, MaxAmount = 500_000_000, RepaymentMonths = 60,
                Requirements = "پیش‌فاکتور خودرو، فیش حقوقی، بیمه‌نامه",
                IsActive = true, ViewCount = 1750, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.sb24.ir/personal/facilities"
            },
            new Loan
            {
                Id = 36, Title = "وام خرید کالا بانک سامان", Slug = "vam-kala-bank-saman", BankId = 12, LoanTypeId = 6,
                ShortDescription = "وام خرید لوازم خانگی تا ۲۵۰ میلیون تومان",
                FullDescription = "بانک سامان تسهیلات خرید کالا از فروشگاه‌های طرف قرارداد ارائه می‌دهد.\n\n• سقف: ۲۵۰ میلیون تومان\n• سود: ۲۳ درصد\n• بازپرداخت: ۳۶ ماه\n\nمزایا:\n• شبکه گسترده فروشگاه‌های طرف قرارداد\n• صدور آنلاین",
                InterestRate = 23, MaxAmount = 250_000_000, RepaymentMonths = 36,
                Requirements = "فاکتور فروشگاه طرف قرارداد، فیش حقوقی",
                IsActive = true, ViewCount = 920, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.sb24.ir/personal/facilities"
            },

            // ────────────────────── بانک اقتصاد نوین (13) ──────────────────────
            new Loan
            {
                Id = 37, Title = "وام ازدواج بانک اقتصاد نوین", Slug = "vam-ezdevaj-eghtesad-novin", BankId = 13, LoanTypeId = 2,
                ShortDescription = "وام ازدواج ۳۰۰ میلیون تومانی بانک اقتصاد نوین",
                FullDescription = "بانک اقتصاد نوین وام ازدواج قرض‌الحسنه ارائه می‌دهد.\n\nمبلغ: ۳۰۰ تا ۳۵۰ میلیون تومان\nکارمزد: ۴ درصد\nبازپرداخت: ۱۲۰ ماه",
                InterestRate = 4, MinAmount = 300_000_000, MaxAmount = 350_000_000, RepaymentMonths = 120,
                Requirements = "عقدنامه رسمی، مدارک شناسایی، ضامن",
                IsActive = true, ViewCount = 1650, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.enbank.ir/facilities"
            },
            new Loan
            {
                Id = 38, Title = "وام مسکن بانک اقتصاد نوین", Slug = "vam-maskan-eghtesad-novin", BankId = 13, LoanTypeId = 1,
                ShortDescription = "تسهیلات خرید مسکن تا ۵۰۰ میلیون تومان",
                FullDescription = "بانک اقتصاد نوین تسهیلات خرید مسکن ارائه می‌دهد.\n\n• سقف: ۵۰۰ میلیون تومان\n• سود: ۱۸ درصد\n• بازپرداخت: ۱۲۰ ماه\n• وثیقه: سند ملک",
                InterestRate = 18, MaxAmount = 500_000_000, RepaymentMonths = 120,
                Requirements = "مبایعه‌نامه، ارزیابی ملک، ضامن",
                IsActive = true, ViewCount = 1300, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.enbank.ir/facilities"
            },
            new Loan
            {
                Id = 39, Title = "وام سرمایه در گردش اقتصاد نوین", Slug = "vam-sarmayeh-eghtesad-novin", BankId = 13, LoanTypeId = 5,
                ShortDescription = "تسهیلات سرمایه در گردش تا ۲ میلیارد تومان",
                FullDescription = "بانک اقتصاد نوین تسهیلات سرمایه در گردش برای بنگاه‌های اقتصادی ارائه می‌دهد.\n\n• سقف: ۲ میلیارد تومان\n• سود: ۲۳ درصد\n• بازپرداخت: ۲۴ ماه",
                InterestRate = 23, MaxAmount = 2_000_000_000, RepaymentMonths = 24,
                Requirements = "جواز کسب، صورت‌های مالی، وثیقه ملکی",
                IsActive = true, ViewCount = 870, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.enbank.ir/facilities"
            },

            // ────────────────────── بانک کارآفرین (14) ──────────────────────
            new Loan
            {
                Id = 40, Title = "وام ازدواج بانک کارآفرین", Slug = "vam-ezdevaj-bank-karafarin", BankId = 14, LoanTypeId = 2,
                ShortDescription = "وام ازدواج ۳۰۰ میلیون تومانی بانک کارآفرین",
                FullDescription = "بانک کارآفرین وام ازدواج قرض‌الحسنه ارائه می‌دهد.\n\nمبلغ: ۳۰۰ تا ۳۵۰ میلیون تومان\nکارمزد: ۴ درصد\nبازپرداخت: ۱۲۰ ماه",
                InterestRate = 4, MinAmount = 300_000_000, MaxAmount = 350_000_000, RepaymentMonths = 120,
                Requirements = "عقدنامه رسمی، مدارک شناسایی، ضامن",
                IsActive = true, ViewCount = 780, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.karafarinbank.ir/facilities"
            },
            new Loan
            {
                Id = 41, Title = "تسهیلات کارآفرینی و SME", Slug = "vam-karafarini-sme", BankId = 14, LoanTypeId = 5,
                ShortDescription = "تسهیلات کارآفرینی و بنگاه‌های کوچک تا ۳ میلیارد تومان",
                FullDescription = "بانک کارآفرین تسهیلات ویژه بنگاه‌های کوچک و متوسط (SME) و استارت‌آپ‌ها ارائه می‌دهد.\n\n• سقف: ۳ میلیارد تومان\n• سود: ۲۳ درصد\n• بازپرداخت: ۳۶ تا ۶۰ ماه\n\nحوزه‌ها:\n• سرمایه در گردش\n• خرید تجهیزات\n• تأمین مالی پروژه\n• توسعه کسب‌وکار",
                InterestRate = 23, MaxAmount = 3_000_000_000, RepaymentMonths = 60,
                Requirements = "طرح توجیهی، جواز کسب، صورت‌های مالی، وثیقه",
                IsActive = true, IsFeatured = true, ViewCount = 2200, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.karafarinbank.ir/facilities"
            },

            // ────────────────────── بانک سینا (15) ──────────────────────
            new Loan
            {
                Id = 42, Title = "وام ازدواج بانک سینا", Slug = "vam-ezdevaj-bank-sina", BankId = 15, LoanTypeId = 2,
                ShortDescription = "وام ازدواج ۳۰۰ میلیون تومانی بانک سینا",
                FullDescription = "بانک سینا وام ازدواج قرض‌الحسنه ارائه می‌دهد.\n\nمبلغ: ۳۰۰ تا ۳۵۰ میلیون تومان\nکارمزد: ۴ درصد\nبازپرداخت: ۱۲۰ ماه",
                InterestRate = 4, MinAmount = 300_000_000, MaxAmount = 350_000_000, RepaymentMonths = 120,
                Requirements = "عقدنامه رسمی، مدارک شناسایی، ضامن",
                IsActive = true, ViewCount = 690, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.sinabank.ir/facilities"
            },
            new Loan
            {
                Id = 43, Title = "وام خرید کالا بانک سینا", Slug = "vam-kala-bank-sina", BankId = 15, LoanTypeId = 6,
                ShortDescription = "وام خرید لوازم خانگی تا ۱۵۰ میلیون تومان",
                FullDescription = "بانک سینا تسهیلات خرید کالا ارائه می‌دهد.\n\n• سقف: ۱۵۰ میلیون تومان\n• سود: ۲۳ درصد\n• بازپرداخت: ۲۴ ماه",
                InterestRate = 23, MaxAmount = 150_000_000, RepaymentMonths = 24,
                Requirements = "فاکتور فروشگاه طرف قرارداد، فیش حقوقی",
                IsActive = true, ViewCount = 430, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.sinabank.ir/facilities"
            },
            new Loan
            {
                Id = 44, Title = "قرض‌الحسنه بانک سینا", Slug = "gharz-hasaneh-bank-sina", BankId = 15, LoanTypeId = 7,
                ShortDescription = "قرض‌الحسنه تا ۱۰۰ میلیون تومان",
                FullDescription = "بانک سینا تسهیلات قرض‌الحسنه ارائه می‌دهد.\n\n• سقف: ۱۰۰ میلیون تومان\n• کارمزد: ۴ درصد\n• بازپرداخت: ۳۶ ماه",
                InterestRate = 4, MaxAmount = 100_000_000, RepaymentMonths = 36,
                Requirements = "حساب قرض‌الحسنه فعال، ضامن",
                IsActive = true, ViewCount = 310, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.sinabank.ir/facilities"
            },

            // ────────────────────── بانک شهر (16) ──────────────────────
            new Loan
            {
                Id = 45, Title = "وام ازدواج بانک شهر", Slug = "vam-ezdevaj-bank-shahr", BankId = 16, LoanTypeId = 2,
                ShortDescription = "وام ازدواج ۳۰۰ میلیون تومانی بانک شهر",
                FullDescription = "بانک شهر وام ازدواج قرض‌الحسنه ارائه می‌دهد.\n\nمبلغ: ۳۰۰ تا ۳۵۰ میلیون تومان\nکارمزد: ۴ درصد\nبازپرداخت: ۱۲۰ ماه",
                InterestRate = 4, MinAmount = 300_000_000, MaxAmount = 350_000_000, RepaymentMonths = 120,
                Requirements = "عقدنامه رسمی، مدارک شناسایی، ضامن",
                IsActive = true, ViewCount = 820, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.shahr-bank.ir/facilities"
            },
            new Loan
            {
                Id = 46, Title = "وام مسکن بانک شهر", Slug = "vam-maskan-bank-shahr", BankId = 16, LoanTypeId = 1,
                ShortDescription = "تسهیلات خرید مسکن تا ۴۰۰ میلیون تومان",
                FullDescription = "بانک شهر تسهیلات خرید مسکن ارائه می‌دهد.\n\n• سقف: ۴۰۰ میلیون تومان\n• سود: ۱۸ درصد\n• بازپرداخت: ۱۲۰ ماه",
                InterestRate = 18, MaxAmount = 400_000_000, RepaymentMonths = 120,
                Requirements = "مبایعه‌نامه، ارزیابی ملک، ضامن",
                IsActive = true, ViewCount = 1100, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.shahr-bank.ir/facilities"
            },
            new Loan
            {
                Id = 47, Title = "وام شهروندی بانک شهر", Slug = "vam-shahrvandi-bank-shahr", BankId = 16, LoanTypeId = 7,
                ShortDescription = "تسهیلات شهروندی تا ۱۵۰ میلیون تومان",
                FullDescription = "بانک شهر تسهیلات شهروندی ویژه ساکنین شهرها ارائه می‌دهد.\n\n• سقف: ۱۵۰ میلیون تومان\n• سود: ۱۸ درصد\n• بازپرداخت: ۳۶ ماه\n\nکاربردها:\n• رفع نیازهای ضروری\n• تعمیرات منزل\n• هزینه‌های درمانی\n• تحصیلی",
                InterestRate = 18, MaxAmount = 150_000_000, RepaymentMonths = 36,
                Requirements = "سکونت در شهر، فیش حقوقی، ضامن",
                IsActive = true, ViewCount = 650, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.shahr-bank.ir/facilities"
            },

            // ────────────────────── بانک گردشگری (17) ──────────────────────
            new Loan
            {
                Id = 48, Title = "وام ازدواج بانک گردشگری", Slug = "vam-ezdevaj-bank-gardeshgari", BankId = 17, LoanTypeId = 2,
                ShortDescription = "وام ازدواج ۳۰۰ میلیون تومانی بانک گردشگری",
                FullDescription = "بانک گردشگری وام ازدواج قرض‌الحسنه ارائه می‌دهد.\n\nمبلغ: ۳۰۰ تا ۳۵۰ میلیون تومان\nکارمزد: ۴ درصد\nبازپرداخت: ۱۲۰ ماه",
                InterestRate = 4, MinAmount = 300_000_000, MaxAmount = 350_000_000, RepaymentMonths = 120,
                Requirements = "عقدنامه رسمی، مدارک شناسایی، ضامن",
                IsActive = true, ViewCount = 560, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.tourismbank.ir/facilities"
            },
            new Loan
            {
                Id = 49, Title = "تسهیلات گردشگری و هتلداری", Slug = "vam-gardeshgari-hoteldar", BankId = 17, LoanTypeId = 5,
                ShortDescription = "تسهیلات صنعت گردشگری و هتلداری تا ۱۰ میلیارد تومان",
                FullDescription = "بانک گردشگری تسهیلات ویژه توسعه صنعت گردشگری ارائه می‌دهد.\n\n• سقف: ۱۰ میلیارد تومان\n• سود: ۱۸ درصد\n• بازپرداخت: ۶۰ تا ۱۲۰ ماه\n\nحوزه‌ها:\n• ساخت و توسعه هتل\n• اقامتگاه‌های بوم‌گردی\n• آژانس‌های مسافرتی\n• رستوران و سفره‌خانه\n• حمل‌ونقل گردشگری",
                InterestRate = 18, MaxAmount = 10_000_000_000, RepaymentMonths = 120,
                Requirements = "مجوز میراث فرهنگی، طرح توجیهی، وثیقه ملکی",
                IsActive = true, IsFeatured = true, ViewCount = 3800, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.tourismbank.ir/facilities"
            },

            // ────────────────────── بانک خاورمیانه (18) ──────────────────────
            new Loan
            {
                Id = 50, Title = "وام ازدواج بانک خاورمیانه", Slug = "vam-ezdevaj-bank-khavarmianeh", BankId = 18, LoanTypeId = 2,
                ShortDescription = "وام ازدواج ۳۰۰ میلیون تومانی بانک خاورمیانه",
                FullDescription = "بانک خاورمیانه وام ازدواج قرض‌الحسنه ارائه می‌دهد.\n\nمبلغ: ۳۰۰ تا ۳۵۰ میلیون تومان\nکارمزد: ۴ درصد\nبازپرداخت: ۱۲۰ ماه",
                InterestRate = 4, MinAmount = 300_000_000, MaxAmount = 350_000_000, RepaymentMonths = 120,
                Requirements = "عقدنامه رسمی، مدارک شناسایی، ضامن",
                IsActive = true, ViewCount = 420, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.mebank.ir/facilities"
            },
            new Loan
            {
                Id = 51, Title = "وام خودرو بانک خاورمیانه", Slug = "vam-khodro-bank-khavarmianeh", BankId = 18, LoanTypeId = 3,
                ShortDescription = "وام خرید خودرو تا ۴۰۰ میلیون تومان",
                FullDescription = "بانک خاورمیانه تسهیلات خرید خودرو ارائه می‌دهد.\n\n• سقف: ۴۰۰ میلیون تومان\n• سود: ۲۳ درصد\n• بازپرداخت: ۶۰ ماه",
                InterestRate = 23, MaxAmount = 400_000_000, RepaymentMonths = 60,
                Requirements = "پیش‌فاکتور خودرو، فیش حقوقی، ضامن",
                IsActive = true, ViewCount = 380, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.mebank.ir/facilities"
            },

            // ────────────────────── بانک قرض‌الحسنه مهر ایران (19) ──────────────────────
            new Loan
            {
                Id = 52, Title = "وام ازدواج مهر ایران", Slug = "vam-ezdevaj-bank-mehr", BankId = 19, LoanTypeId = 2,
                ShortDescription = "وام ازدواج ۳۰۰ میلیون تومانی بانک مهر ایران",
                FullDescription = "بانک قرض‌الحسنه مهر ایران وام ازدواج قرض‌الحسنه ارائه می‌دهد.\n\nمبلغ: ۳۰۰ تا ۳۵۰ میلیون تومان\nکارمزد: ۴ درصد\nبازپرداخت: ۱۲۰ ماه\n\nمزایا:\n• تخصص در تسهیلات قرض‌الحسنه\n• فرآیند ساده‌تر نسبت به بانک‌های تجاری\n• شعبات گسترده",
                InterestRate = 4, MinAmount = 300_000_000, MaxAmount = 350_000_000, RepaymentMonths = 120,
                Requirements = "عقدنامه رسمی، مدارک شناسایی، ضامن",
                IsActive = true, IsFeatured = true, ViewCount = 6700, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.qmb.ir/facilities"
            },
            new Loan
            {
                Id = 53, Title = "قرض‌الحسنه ضروری مهر ایران", Slug = "gharz-zarori-bank-mehr", BankId = 19, LoanTypeId = 7,
                ShortDescription = "قرض‌الحسنه ضروری تا ۲۰۰ میلیون تومان",
                FullDescription = "بانک مهر ایران تسهیلات قرض‌الحسنه ضروری ارائه می‌دهد.\n\n• سقف: ۲۰۰ میلیون تومان\n• کارمزد: ۴ درصد\n• بازپرداخت: ۴۸ ماه\n\nکاربردها:\n• هزینه‌های درمانی\n• تعمیر مسکن\n• تحصیل فرزندان\n• رفع مشکلات ضروری",
                InterestRate = 4, MaxAmount = 200_000_000, RepaymentMonths = 48,
                Requirements = "حساب قرض‌الحسنه فعال، سپرده‌گذاری، ضامن",
                IsActive = true, IsFeatured = true, ViewCount = 5400, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.qmb.ir/facilities"
            },
            new Loan
            {
                Id = 54, Title = "قرض‌الحسنه اشتغال‌زایی مهر ایران", Slug = "gharz-eshteghal-bank-mehr", BankId = 19, LoanTypeId = 5,
                ShortDescription = "قرض‌الحسنه اشتغال‌زایی تا ۳۰۰ میلیون تومان",
                FullDescription = "بانک مهر ایران تسهیلات قرض‌الحسنه اشتغال‌زایی ارائه می‌دهد.\n\n• سقف: ۳۰۰ میلیون تومان\n• کارمزد: ۴ درصد\n• بازپرداخت: ۶۰ ماه\n\nشرایط:\n• ارائه طرح توجیهی\n• جواز کسب یا مجوز فعالیت\n• ضامن معتبر",
                InterestRate = 4, MaxAmount = 300_000_000, RepaymentMonths = 60,
                Requirements = "طرح توجیهی، جواز کسب، ضامن",
                IsActive = true, ViewCount = 3100, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.qmb.ir/facilities"
            },

            // ────────────────────── بانک قرض‌الحسنه رسالت (20) ──────────────────────
            new Loan
            {
                Id = 55, Title = "وام ازدواج بانک رسالت", Slug = "vam-ezdevaj-bank-resalat", BankId = 20, LoanTypeId = 2,
                ShortDescription = "وام ازدواج ۳۰۰ میلیون تومانی بانک رسالت",
                FullDescription = "بانک قرض‌الحسنه رسالت وام ازدواج قرض‌الحسنه ارائه می‌دهد.\n\nمبلغ: ۳۰۰ تا ۳۵۰ میلیون تومان\nکارمزد: ۴ درصد\nبازپرداخت: ۱۲۰ ماه",
                InterestRate = 4, MinAmount = 300_000_000, MaxAmount = 350_000_000, RepaymentMonths = 120,
                Requirements = "عقدنامه رسمی، مدارک شناسایی، ضامن",
                IsActive = true, ViewCount = 1100, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.resalatbank.ir/facilities"
            },
            new Loan
            {
                Id = 56, Title = "قرض‌الحسنه بانک رسالت", Slug = "gharz-hasaneh-bank-resalat", BankId = 20, LoanTypeId = 7,
                ShortDescription = "قرض‌الحسنه تا ۱۵۰ میلیون تومان",
                FullDescription = "بانک رسالت تسهیلات قرض‌الحسنه ارائه می‌دهد.\n\n• سقف: ۱۵۰ میلیون تومان\n• کارمزد: ۴ درصد\n• بازپرداخت: ۳۶ ماه\n\nمزایا:\n• رویکرد تعاونی و مردم‌محور\n• فرآیند ساده\n• حداقل بروکراسی",
                InterestRate = 4, MaxAmount = 150_000_000, RepaymentMonths = 36,
                Requirements = "حساب قرض‌الحسنه فعال، ضامن",
                IsActive = true, ViewCount = 890, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.resalatbank.ir/facilities"
            },
            new Loan
            {
                Id = 57, Title = "وام خرید کالا بانک رسالت", Slug = "vam-kala-bank-resalat", BankId = 20, LoanTypeId = 6,
                ShortDescription = "وام خرید کالا تا ۱۰۰ میلیون تومان",
                FullDescription = "بانک رسالت تسهیلات خرید کالا ارائه می‌دهد.\n\n• سقف: ۱۰۰ میلیون تومان\n• سود: ۴ درصد (قرض‌الحسنه)\n• بازپرداخت: ۲۴ ماه\n\nمزایا:\n• نرخ بسیار پایین به صورت قرض‌الحسنه\n• خرید از فروشگاه‌های طرف قرارداد",
                InterestRate = 4, MaxAmount = 100_000_000, RepaymentMonths = 24,
                Requirements = "فاکتور فروشگاه، حساب فعال، ضامن",
                IsActive = true, ViewCount = 670, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.resalatbank.ir/facilities"
            },

            // ────────────────────── وام‌های آنلاین و بدون ضامن (نئوبانک‌ها) ──────────────────────

            // بلوبانک (23)
            new Loan
            {
                Id = 58, Title = "وام آنلاین بلوبانک", Slug = "vam-online-blubank", BankId = 23, LoanTypeId = 8,
                ShortDescription = "وام آنلاین بلوبانک تا ۱۰۰ میلیون تومان بدون مراجعه حضوری",
                FullDescription = "بلوبانک وام‌های آنلاین را بدون نیاز به مراجعه حضوری به شعبه ارائه می‌دهد.\n\nویژگی‌ها:\n• درخواست کاملاً آنلاین از اپلیکیشن\n• سقف: ۱۰۰ میلیون تومان\n• بررسی اعتبار لحظه‌ای\n• واریز سریع به حساب\n• بازپرداخت: ۱۲ تا ۲۴ ماه\n\nشرایط:\n• داشتن حساب فعال بلوبانک\n• اعتبارسنجی مثبت\n• حداقل ۳ ماه سابقه حساب",
                InterestRate = 23, MaxAmount = 100_000_000, RepaymentMonths = 24,
                Requirements = "حساب فعال بلوبانک، اعتبارسنجی مثبت",
                IsActive = true, IsFeatured = true, ViewCount = 12000, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://blubank.com/loans"
            },
            new Loan
            {
                Id = 59, Title = "وام بدون ضامن بلوبانک", Slug = "vam-bedone-zamen-blubank", BankId = 23, LoanTypeId = 9,
                ShortDescription = "وام بدون ضامن بلوبانک تا ۵۰ میلیون تومان",
                FullDescription = "بلوبانک وام بدون ضامن را برای مشتریان خوش‌حساب ارائه می‌دهد.\n\nویژگی‌ها:\n• بدون نیاز به ضامن\n• سقف: ۵۰ میلیون تومان\n• فرآیند ساده و سریع\n• بازپرداخت: ۱۲ ماه\n\nشرایط:\n• سابقه حساب حداقل ۶ ماه\n• گردش حساب مناسب\n• عدم بدهی معوق",
                InterestRate = 23, MaxAmount = 50_000_000, RepaymentMonths = 12,
                Requirements = "حساب فعال بلوبانک با سابقه ۶ ماهه، بدون بدهی معوق",
                IsActive = true, IsFeatured = true, ViewCount = 9500, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://blubank.com/loans"
            },
            new Loan
            {
                Id = 60, Title = "وام فوری بلوبانک", Slug = "vam-fori-blubank", BankId = 23, LoanTypeId = 10,
                ShortDescription = "وام فوری بلوبانک تا ۳۰ میلیون تومان در کمتر از ۲۴ ساعت",
                FullDescription = "بلوبانک وام فوری را با پردازش سریع ارائه می‌دهد.\n\nویژگی‌ها:\n• پردازش و واریز در کمتر از ۲۴ ساعت\n• سقف: ۳۰ میلیون تومان\n• بدون مراجعه حضوری\n• بازپرداخت: ۶ تا ۱۲ ماه",
                InterestRate = 23, MaxAmount = 30_000_000, RepaymentMonths = 12,
                Requirements = "حساب فعال بلوبانک، اعتبارسنجی مثبت",
                IsActive = true, ViewCount = 7800, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://blubank.com/loans"
            },

            // هپی (21)
            new Loan
            {
                Id = 61, Title = "وام آنلاین هپی بانک", Slug = "vam-online-hibank", BankId = 21, LoanTypeId = 8,
                ShortDescription = "وام آنلاین هپی تا ۸۰ میلیون تومان از اپلیکیشن",
                FullDescription = "هپی (HiBank) وام آنلاین را بدون مراجعه حضوری ارائه می‌دهد.\n\nویژگی‌ها:\n• درخواست از اپلیکیشن هپی\n• سقف: ۸۰ میلیون تومان\n• بازپرداخت: ۱۲ تا ۲۴ ماه\n• اعتبارسنجی هوشمند",
                InterestRate = 23, MaxAmount = 80_000_000, RepaymentMonths = 24,
                Requirements = "حساب فعال هپی بانک، اعتبارسنجی مثبت",
                IsActive = true, ViewCount = 4500, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://hibank.ir/loans"
            },
            new Loan
            {
                Id = 62, Title = "وام بدون ضامن هپی", Slug = "vam-bedone-zamen-hibank", BankId = 21, LoanTypeId = 9,
                ShortDescription = "وام بدون ضامن هپی تا ۴۰ میلیون تومان",
                FullDescription = "هپی بانک وام بدون ضامن ارائه می‌دهد.\n\n• سقف: ۴۰ میلیون تومان\n• بدون نیاز به ضامن\n• بازپرداخت: ۱۲ ماه\n• فقط با اعتبارسنجی مثبت",
                InterestRate = 23, MaxAmount = 40_000_000, RepaymentMonths = 12,
                Requirements = "حساب فعال هپی، سابقه حداقل ۳ ماه",
                IsActive = true, ViewCount = 3200, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://hibank.ir/loans"
            },

            // بانکینو (25)
            new Loan
            {
                Id = 63, Title = "وام آنلاین بانکینو", Slug = "vam-online-bankino", BankId = 25, LoanTypeId = 8,
                ShortDescription = "وام آنلاین بانکینو تا ۵۰ میلیون تومان",
                FullDescription = "بانکینو وام آنلاین با فرآیند کاملاً دیجیتال ارائه می‌دهد.\n\nویژگی‌ها:\n• ثبت درخواست از اپلیکیشن\n• سقف: ۵۰ میلیون تومان\n• بازپرداخت: ۱۲ ماه\n• بررسی سریع اعتبار",
                InterestRate = 23, MaxAmount = 50_000_000, RepaymentMonths = 12,
                Requirements = "حساب فعال بانکینو، اعتبارسنجی مثبت",
                IsActive = true, ViewCount = 2800, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://bankino.ir/services"
            },
            new Loan
            {
                Id = 64, Title = "وام فوری بانکینو", Slug = "vam-fori-bankino", BankId = 25, LoanTypeId = 10,
                ShortDescription = "وام فوری بانکینو تا ۲۰ میلیون تومان در چند ساعت",
                FullDescription = "بانکینو وام فوری با واریز سریع ارائه می‌دهد.\n\n• سقف: ۲۰ میلیون تومان\n• واریز در چند ساعت\n• بدون مراجعه حضوری\n• بازپرداخت: ۶ تا ۱۲ ماه",
                InterestRate = 23, MaxAmount = 20_000_000, RepaymentMonths = 12,
                Requirements = "حساب فعال بانکینو، اعتبارسنجی مثبت",
                IsActive = true, ViewCount = 1900, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://bankino.ir/services"
            },

            // وام آنلاین بانک سامان (12)
            new Loan
            {
                Id = 65, Title = "وام آنلاین بانک سامان", Slug = "vam-online-bank-saman", BankId = 12, LoanTypeId = 8,
                ShortDescription = "وام آنلاین بانک سامان تا ۲۰۰ میلیون تومان از سامانه اینترنتی",
                FullDescription = "بانک سامان تسهیلات آنلاین از طریق سامانه اینترنتی ارائه می‌دهد.\n\nویژگی‌ها:\n• درخواست آنلاین از sb24.ir\n• سقف: ۲۰۰ میلیون تومان\n• انواع تسهیلات: خرید کالا، خودرو، نقدی\n• بازپرداخت: تا ۳۶ ماه",
                InterestRate = 23, MaxAmount = 200_000_000, RepaymentMonths = 36,
                Requirements = "حساب فعال بانک سامان، اعتبارسنجی مثبت، ضامن برای مبالغ بالا",
                IsActive = true, IsFeatured = true, ViewCount = 5600, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.sb24.ir/personal/facilities"
            },

            // وام آنلاین بانک پاسارگاد (6)
            new Loan
            {
                Id = 66, Title = "وام آنلاین بانک پاسارگاد", Slug = "vam-online-bank-pasargad", BankId = 6, LoanTypeId = 8,
                ShortDescription = "وام آنلاین بانک پاسارگاد تا ۱۵۰ میلیون تومان",
                FullDescription = "بانک پاسارگاد تسهیلات آنلاین از طریق اپلیکیشن موبایل ارائه می‌دهد.\n\nویژگی‌ها:\n• درخواست از اپلیکیشن بانک پاسارگاد\n• سقف: ۱۵۰ میلیون تومان\n• اعتبارسنجی هوشمند\n• بازپرداخت: تا ۲۴ ماه",
                InterestRate = 23, MaxAmount = 150_000_000, RepaymentMonths = 24,
                Requirements = "حساب فعال بانک پاسارگاد، اعتبارسنجی مثبت",
                IsActive = true, ViewCount = 4200, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.bpi.ir/fa/personal-banking/facilities"
            },

            // تسهیلات جعاله بانک مسکن (8)
            new Loan
            {
                Id = 67, Title = "تسهیلات جعاله بانک مسکن", Slug = "joaleh-bank-maskan", BankId = 8, LoanTypeId = 11,
                ShortDescription = "تسهیلات جعاله برای تعمیرات مسکن تا ۲۰۰ میلیون تومان",
                FullDescription = "بانک مسکن تسهیلات جعاله برای تعمیرات و بازسازی مسکن ارائه می‌دهد.\n\nویژگی‌ها:\n• سقف: ۲۰۰ میلیون تومان\n• نرخ سود: ۱۸ درصد\n• بازپرداخت: ۶۰ ماه\n• کاربرد: تعمیرات اساسی، تغییر کاربری، بازسازی",
                InterestRate = 18, MaxAmount = 200_000_000, RepaymentMonths = 60,
                Requirements = "سند مالکیت، برآورد هزینه تعمیرات، ارزیابی ملک",
                IsActive = true, ViewCount = 1800, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.bank-maskan.ir/fa/facilities"
            },

            // وام ودیعه مسکن بانک ملی (1)
            new Loan
            {
                Id = 68, Title = "وام ودیعه مسکن بانک ملی", Slug = "vam-odieh-maskan-bank-melli", BankId = 1, LoanTypeId = 12,
                ShortDescription = "وام ودیعه مسکن بانک ملی تا ۲۷۵ میلیون تومان",
                FullDescription = "بانک ملی ایران وام ودیعه مسکن مستأجران ارائه می‌دهد.\n\nسقف بر اساس شهر:\n• تهران: ۲۷۵ میلیون تومان\n• کلانشهرها: ۲۱۰ میلیون تومان\n• سایر شهرها: ۱۴۰ میلیون تومان\n\n• سود: ۲۳ درصد\n• بازپرداخت: ۳۶ ماه",
                InterestRate = 23, MinAmount = 100_000_000, MaxAmount = 275_000_000, RepaymentMonths = 36,
                Requirements = "اجاره‌نامه رسمی، مدارک شناسایی، ضامن یا سفته",
                IsActive = true, ViewCount = 3600, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.bmi.ir/fa/facility-services"
            },

            // وام فوری بانک ملت (2)
            new Loan
            {
                Id = 69, Title = "وام فوری بانک ملت", Slug = "vam-fori-bank-mellat", BankId = 2, LoanTypeId = 10,
                ShortDescription = "وام فوری بانک ملت تا ۱۰۰ میلیون تومان از سامانه بام",
                FullDescription = "بانک ملت وام فوری از طریق سامانه بام (بانکداری الکترونیک) ارائه می‌دهد.\n\nویژگی‌ها:\n• درخواست از سامانه بام\n• سقف: ۱۰۰ میلیون تومان\n• پردازش سریع\n• بازپرداخت: ۱۲ تا ۲۴ ماه\n• نرخ سود: ۲۳ درصد",
                InterestRate = 23, MaxAmount = 100_000_000, RepaymentMonths = 24,
                Requirements = "حساب فعال بانک ملت، اعتبارسنجی مثبت",
                IsActive = true, ViewCount = 4800, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.bankmellat.ir/facilities"
            },

            // وام تحصیلی (جدید)
            new Loan
            {
                Id = 70, Title = "وام تحصیلی بانک ملی", Slug = "vam-tahsili-bank-melli", BankId = 1, LoanTypeId = 4,
                ShortDescription = "وام تحصیلی بانک ملی برای دانشجویان تا ۵۰ میلیون تومان",
                FullDescription = "بانک ملی ایران تسهیلات تحصیلی برای دانشجویان مقاطع مختلف ارائه می‌دهد.\n\nویژگی‌ها:\n• سقف: ۵۰ میلیون تومان\n• نرخ سود: ۴ درصد (قرض‌الحسنه)\n• بازپرداخت: پس از فارغ‌التحصیلی\n• مدت بازپرداخت: ۶۰ ماه\n\nشرایط:\n• دانشجوی مقاطع کاردانی تا دکتری\n• دانشگاه مورد تأیید وزارت علوم",
                InterestRate = 4, MaxAmount = 50_000_000, RepaymentMonths = 60,
                Requirements = "گواهی اشتغال به تحصیل، مدارک شناسایی، ضامن",
                IsActive = true, ViewCount = 2200, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.bmi.ir/fa/facility-services"
            },
            new Loan
            {
                Id = 71, Title = "وام تحصیلی بانک ملت", Slug = "vam-tahsili-bank-mellat", BankId = 2, LoanTypeId = 4,
                ShortDescription = "وام تحصیلی بانک ملت تا ۴۰ میلیون تومان",
                FullDescription = "بانک ملت تسهیلات تحصیلی ارائه می‌دهد.\n\n• سقف: ۴۰ میلیون تومان\n• نرخ سود: ۴ درصد\n• بازپرداخت: پس از فارغ‌التحصیلی\n• مدت: ۶۰ ماه",
                InterestRate = 4, MaxAmount = 40_000_000, RepaymentMonths = 60,
                Requirements = "گواهی اشتغال به تحصیل، ضامن",
                IsActive = true, ViewCount = 1800, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.bankmellat.ir/facilities"
            },

            // تسهیلات جعاله بانک ملی (1)
            new Loan
            {
                Id = 72, Title = "تسهیلات جعاله بانک ملی", Slug = "joaleh-bank-melli", BankId = 1, LoanTypeId = 11,
                ShortDescription = "تسهیلات جعاله بانک ملی تا ۳۰۰ میلیون تومان",
                FullDescription = "بانک ملی ایران تسهیلات جعاله برای تعمیرات، خدمات و پروژه‌ها ارائه می‌دهد.\n\nویژگی‌ها:\n• سقف: ۳۰۰ میلیون تومان\n• نرخ سود: ۲۳ درصد\n• بازپرداخت: ۳۶ ماه\n• کاربرد: تعمیرات ساختمان، تجهیزات، خدمات",
                InterestRate = 23, MaxAmount = 300_000_000, RepaymentMonths = 36,
                Requirements = "برآورد هزینه، فیش حقوقی، ضامن",
                IsActive = true, ViewCount = 950, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.bmi.ir/fa/facility-services"
            },

            // ────────────────────── نئوبانک‌ها (وام‌های جدید) ──────────────────────
            // ویپاد (22)
            new Loan
            {
                Id = 73, Title = "وام پرداخت آنلاین ویپاد", Slug = "vam-pardakht-online-vipad", BankId = 22, LoanTypeId = 8,
                ShortDescription = "وام پرداخت آنلاین ویپاد تا ۵۰ میلیون تومان",
                FullDescription = "ویپاد وام پرداخت آنلاین را بدون مراجعه حضوری ارائه می‌دهد.\n\nویژگی‌ها:\n• درخواست از اپلیکیشن ویپاد\n• سقف: ۵۰ میلیون تومان\n• بازپرداخت: ۱۲ تا ۲۴ ماه\n• اعتبارسنجی آنلاین",
                InterestRate = 23, MaxAmount = 50_000_000, RepaymentMonths = 24,
                Requirements = "حساب فعال ویپاد، اعتبارسنجی مثبت",
                IsActive = true, ViewCount = 0, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.vipad.ir"
            },
            new Loan
            {
                Id = 74, Title = "وام کیف پول دیجیتال ویپاد", Slug = "vam-kifpol-digital-vipad", BankId = 22, LoanTypeId = 10,
                ShortDescription = "وام کیف پول دیجیتال ویپاد تا ۳۰ میلیون تومان",
                FullDescription = "ویپاد وام کیف پول دیجیتال با واریز سریع ارائه می‌دهد.\n\n• سقف: ۳۰ میلیون تومان\n• واریز به کیف پول\n• بازپرداخت: ۶ تا ۱۲ ماه",
                InterestRate = 23, MaxAmount = 30_000_000, RepaymentMonths = 12,
                Requirements = "حساب فعال ویپاد، اعتبارسنجی مثبت",
                IsActive = true, ViewCount = 0, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://www.vipad.ir"
            },
            // سپینو (24)
            new Loan
            {
                Id = 75, Title = "وام آنلاین سپینو", Slug = "vam-online-sepino", BankId = 24, LoanTypeId = 8,
                ShortDescription = "وام آنلاین سپینو تا ۶۰ میلیون تومان",
                FullDescription = "سپینو وام آنلاین با فرآیند دیجیتال ارائه می‌دهد.\n\n• سقف: ۶۰ میلیون تومان\n• بازپرداخت: ۱۲ تا ۲۴ ماه\n• درخواست از اپلیکیشن",
                InterestRate = 23, MaxAmount = 60_000_000, RepaymentMonths = 24,
                Requirements = "حساب فعال سپینو، اعتبارسنجی مثبت",
                IsActive = true, ViewCount = 0, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://sepino.ir"
            },
            new Loan
            {
                Id = 76, Title = "خدمات انتقال و وام سپینو", Slug = "vam-entegal-sepino", BankId = 24, LoanTypeId = 10,
                ShortDescription = "وام و خدمات انتقال سپینو تا ۴۰ میلیون تومان",
                FullDescription = "سپینو وام همراه با خدمات انتقال وجه سریع ارائه می‌دهد.\n\n• سقف: ۴۰ میلیون تومان\n• واریز فوری\n• بازپرداخت: ۱۲ ماه",
                InterestRate = 23, MaxAmount = 40_000_000, RepaymentMonths = 12,
                Requirements = "حساب فعال سپینو، اعتبارسنجی مثبت",
                IsActive = true, ViewCount = 0, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://sepino.ir"
            },
            // ایوا (26)
            new Loan
            {
                Id = 77, Title = "وام سفر ایوا", Slug = "vam-safar-eva", BankId = 26, LoanTypeId = 8,
                ShortDescription = "وام سفر و گردشگری ایوا تا ۵۰ میلیون تومان",
                FullDescription = "ایوا (نشان‌بانک) وام سفر برای مسافران و گردشگران ارائه می‌دهد.\n\n• سقف: ۵۰ میلیون تومان\n• بازپرداخت: ۱۲ تا ۲۴ ماه\n• درخواست آنلاین",
                InterestRate = 23, MaxAmount = 50_000_000, RepaymentMonths = 24,
                Requirements = "حساب فعال ایوا، اعتبارسنجی مثبت",
                IsActive = true, ViewCount = 0, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://neshanbank.ir"
            },
            new Loan
            {
                Id = 78, Title = "وام خدمات آنلاین ایوا", Slug = "vam-khadamat-online-eva", BankId = 26, LoanTypeId = 10,
                ShortDescription = "وام خدمات آنلاین ایوا تا ۳۵ میلیون تومان",
                FullDescription = "ایوا وام خدمات آنلاین با واریز سریع ارائه می‌دهد.\n\n• سقف: ۳۵ میلیون تومان\n• بازپرداخت: ۱۲ ماه\n• فرآیند دیجیتال",
                InterestRate = 23, MaxAmount = 35_000_000, RepaymentMonths = 12,
                Requirements = "حساب فعال ایوا، اعتبارسنجی مثبت",
                IsActive = true, ViewCount = 0, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://neshanbank.ir"
            },
            // همراه‌شهر پلاس (27)
            new Loan
            {
                Id = 79, Title = "وام خدمات شهری همراه‌شهر پلاس", Slug = "vam-khadamat-shahri-hamrahshahr", BankId = 27, LoanTypeId = 8,
                ShortDescription = "وام خدمات شهری همراه‌شهر پلاس تا ۷۰ میلیون تومان",
                FullDescription = "همراه‌شهر پلاس وام خدمات شهری برای شهروندان ارائه می‌دهد.\n\n• سقف: ۷۰ میلیون تومان\n• بازپرداخت: ۱۲ تا ۲۴ ماه\n• درخواست از اپلیکیشن",
                InterestRate = 23, MaxAmount = 70_000_000, RepaymentMonths = 24,
                Requirements = "حساب فعال همراه‌شهر پلاس، اعتبارسنجی مثبت",
                IsActive = true, ViewCount = 0, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://hamrahshahrplus.ir"
            },
            // امیدبانک (28)
            new Loan
            {
                Id = 80, Title = "قرض‌الحسنه دیجیتال امیدبانک", Slug = "gharz-digital-omidbank", BankId = 28, LoanTypeId = 7,
                ShortDescription = "قرض‌الحسنه دیجیتال امیدبانک تا ۱۰۰ میلیون تومان",
                FullDescription = "امیدبانک قرض‌الحسنه دیجیتال با کارمزد ۴ درصد ارائه می‌دهد.\n\n• سقف: ۱۰۰ میلیون تومان\n• کارمزد: ۴ درصد\n• بازپرداخت: ۳۶ ماه\n• درخواست از اپلیکیشن",
                InterestRate = 4, MaxAmount = 100_000_000, RepaymentMonths = 36,
                Requirements = "حساب فعال امیدبانک، ضامن",
                IsActive = true, ViewCount = 0, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://omidbank.ir"
            },
            // باجت (29)
            new Loan
            {
                Id = 81, Title = "وام بودجه‌بندی هوشمند باجت", Slug = "vam-budjeh-bajet", BankId = 29, LoanTypeId = 9,
                ShortDescription = "وام بودجه‌بندی هوشمند باجت تا ۵۵ میلیون تومان",
                FullDescription = "باجت وام بودجه‌بندی هوشمند بدون ضامن برای مدیریت مالی شخصی ارائه می‌دهد.\n\n• سقف: ۵۵ میلیون تومان\n• بدون ضامن با اعتبارسنجی\n• بازپرداخت: ۱۲ تا ۲۴ ماه",
                InterestRate = 23, MaxAmount = 55_000_000, RepaymentMonths = 24,
                Requirements = "حساب فعال باجت، اعتبارسنجی مثبت",
                IsActive = true, ViewCount = 0, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://bajet.ir"
            },
            // باران (30)
            new Loan
            {
                Id = 82, Title = "وام کشاورزی دیجیتال باران", Slug = "vam-keshavarzi-digital-baran", BankId = 30, LoanTypeId = 8,
                ShortDescription = "وام کشاورزی دیجیتال باران تا ۸۰ میلیون تومان",
                FullDescription = "باران وام کشاورزی دیجیتال برای کشاورزان و مناطق روستایی ارائه می‌دهد.\n\n• سقف: ۸۰ میلیون تومان\n• بازپرداخت: ۱۲ تا ۳۶ ماه\n• درخواست آنلاین",
                InterestRate = 23, MaxAmount = 80_000_000, RepaymentMonths = 36,
                Requirements = "حساب فعال باران، اعتبارسنجی مثبت",
                IsActive = true, ViewCount = 0, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://baran.bank"
            },
            // بانکت (31)
            new Loan
            {
                Id = 83, Title = "قرض‌الحسنه دیجیتال بانکت", Slug = "gharz-digital-banket", BankId = 31, LoanTypeId = 7,
                ShortDescription = "قرض‌الحسنه دیجیتال بانکت تا ۱۲۰ میلیون تومان",
                FullDescription = "بانکت قرض‌الحسنه دیجیتال با کارمزد ۴ درصد ارائه می‌دهد.\n\n• سقف: ۱۲۰ میلیون تومان\n• کارمزد: ۴ درصد\n• بازپرداخت: ۴۸ ماه\n• درخواست از اپلیکیشن",
                InterestRate = 4, MaxAmount = 120_000_000, RepaymentMonths = 48,
                Requirements = "حساب فعال بانکت، ضامن",
                IsActive = true, ViewCount = 0, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://banket.ir"
            },
            // بانکواره (32)
            new Loan
            {
                Id = 84, Title = "خدمات کشاورزی دیجیتال بانکواره", Slug = "vam-keshavarzi-digital-bankvareh", BankId = 32, LoanTypeId = 8,
                ShortDescription = "وام خدمات کشاورزی دیجیتال بانکواره تا ۷۰ میلیون تومان",
                FullDescription = "بانکواره وام خدمات کشاورزی دیجیتال برای کشاورزان ارائه می‌دهد.\n\n• سقف: ۷۰ میلیون تومان\n• بازپرداخت: ۱۲ تا ۳۶ ماه\n• درخواست آنلاین",
                InterestRate = 23, MaxAmount = 70_000_000, RepaymentMonths = 36,
                Requirements = "حساب فعال بانکواره، اعتبارسنجی مثبت",
                IsActive = true, ViewCount = 0, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://bankvareh.ir"
            },
            // سیبانک (33)
            new Loan
            {
                Id = 85, Title = "وام بانکداری دیجیتال سیبانک", Slug = "vam-bankdari-digital-sibank", BankId = 33, LoanTypeId = 8,
                ShortDescription = "وام بانکداری دیجیتال سیبانک تا ۶۵ میلیون تومان",
                FullDescription = "سیبانک وام بانکداری دیجیتال با فرآیند آنلاین ارائه می‌دهد.\n\n• سقف: ۶۵ میلیون تومان\n• بازپرداخت: ۱۲ تا ۲۴ ماه\n• درخواست از اپلیکیشن",
                InterestRate = 23, MaxAmount = 65_000_000, RepaymentMonths = 24,
                Requirements = "حساب فعال سیبانک، اعتبارسنجی مثبت",
                IsActive = true, ViewCount = 0, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://sibank.ir"
            },
            // فرارفاه (34)
            new Loan
            {
                Id = 86, Title = "وام رفاهی دیجیتال فرارفاه", Slug = "vam-refahi-digital-fararefah", BankId = 34, LoanTypeId = 8,
                ShortDescription = "وام رفاهی دیجیتال فرارفاه تا ۶۰ میلیون تومان",
                FullDescription = "فرارفاه وام رفاهی دیجیتال ویژه حقوق‌بگیران و بازنشستگان ارائه می‌دهد.\n\n• سقف: ۶۰ میلیون تومان\n• بازپرداخت: ۱۲ تا ۲۴ ماه\n• درخواست از اپلیکیشن",
                InterestRate = 23, MaxAmount = 60_000_000, RepaymentMonths = 24,
                Requirements = "حساب فعال فرارفاه، اعتبارسنجی مثبت",
                IsActive = true, ViewCount = 0, CreatedAt = d, UpdatedAt = d,
                ExternalUrl = "https://fararefah.ir"
            }
        );

        // ══════════════════════════════════════════════════════════════════
        //  BLOG POSTS
        // ══════════════════════════════════════════════════════════════════
        modelBuilder.Entity<BlogPost>().HasData(
            new BlogPost
            {
                Id = 1, Title = "راهنمای کامل دریافت وام ازدواج در سال ۱۴۰۴", Slug = "rahnama-vam-ezdevaj-1404",
                Summary = "همه چیز درباره شرایط، مدارک و نحوه ثبت‌نام وام ازدواج در سال ۱۴۰۴. مبلغ، نرخ سود و بانک‌های ارائه‌دهنده.",
                Content = "<h2>وام ازدواج چیست؟</h2><p>وام ازدواج یکی از تسهیلات قرض‌الحسنه است که به زوجین جوان برای تأمین هزینه‌های ازدواج ارائه می‌شود. مبلغ این وام در سال ۱۴۰۴ برای هر یک از زوجین ۳۰۰ میلیون تومان و برای زوجین جوان‌تر تا ۳۵۰ میلیون تومان است.</p><h2>شرایط دریافت</h2><ul><li>تابعیت ایرانی</li><li>عقدنامه رسمی</li><li>عدم دریافت وام ازدواج قبلی</li><li>اعتبارسنجی مثبت</li></ul><h2>مدارک لازم</h2><ul><li>شناسنامه و کارت ملی زوجین</li><li>عقدنامه</li><li>گواهی اشتغال یا فیش حقوقی</li><li>معرفی ضامن معتبر</li></ul><h2>بهترین بانک‌ها برای وام ازدواج</h2><p>تقریباً تمام بانک‌های کشور وام ازدواج ارائه می‌دهند. بانک‌های ملی، ملت، پاسارگاد و مهر ایران از جمله بانک‌هایی هستند که فرآیند سریع‌تری دارند.</p>",
                MetaTitle = "راهنمای کامل وام ازدواج ۱۴۰۴ - شرایط و مدارک",
                MetaDescription = "راهنمای جامع دریافت وام ازدواج در سال ۱۴۰۴. مبلغ ۳۰۰ تا ۳۵۰ میلیون تومان با نرخ ۴ درصد.",
                MetaKeywords = "وام ازدواج, وام ازدواج ۱۴۰۴, شرایط وام ازدواج",
                IsPublished = true, ViewCount = 1250, CreatedAt = d, PublishedAt = d, UpdatedAt = d
            },
            new BlogPost
            {
                Id = 2, Title = "مقایسه وام مسکن بانک‌های مختلف", Slug = "moghayese-vam-maskan",
                Summary = "مقایسه جامع شرایط و نرخ سود وام مسکن در بانک‌های مسکن، ملی، ملت و پاسارگاد.",
                Content = "<h2>وام مسکن در ایران</h2><p>خرید مسکن یکی از بزرگ‌ترین دغدغه‌های خانواده‌های ایرانی است. بانک‌های مختلف تسهیلات خرید مسکن با شرایط متفاوت ارائه می‌دهند.</p><h2>بانک مسکن</h2><p>بانک مسکن به عنوان بانک تخصصی حوزه مسکن، بهترین شرایط را برای وام خرید مسکن ارائه می‌دهد. صندوق یکم با سقف ۶۰۰ میلیون تومان و نرخ ۱۷.۵ درصد یکی از محبوب‌ترین طرح‌هاست.</p><h2>بانک ملی</h2><p>بانک ملی تسهیلات خرید مسکن تا ۸۰۰ میلیون تومان با نرخ ۱۸ درصد ارائه می‌دهد.</p><h2>نتیجه‌گیری</h2><p>برای انتخاب بهترین وام مسکن، باید نرخ سود، مدت بازپرداخت و شرایط هر بانک را با دقت مقایسه کنید.</p>",
                MetaTitle = "مقایسه وام مسکن بانک‌ها - بهترین وام مسکن",
                MetaDescription = "مقایسه کامل وام مسکن بانک‌های مسکن، ملی، ملت و پاسارگاد. نرخ سود و شرایط.",
                MetaKeywords = "وام مسکن, مقایسه وام مسکن, بهترین وام مسکن",
                CoverImageUrl = "/images/blog/vam-maskan.svg",
                IsPublished = true, ViewCount = 890, CreatedAt = d, PublishedAt = d, UpdatedAt = d
            },
            new BlogPost
            {
                Id = 3, Title = "ترابانک چیست؟ آشنایی با نئوبانک‌های ایران", Slug = "trabank-chist",
                Summary = "ترابانک‌ها یا نئوبانک‌ها چه هستند و چه خدماتی ارائه می‌دهند؟ معرفی ترابانک‌های فعال ایران.",
                Content = "<h2>ترابانک (نئوبانک) چیست؟</h2><p>ترابانک یا نئوبانک، بانکی دیجیتال است که تمام خدمات بانکی را از طریق اپلیکیشن موبایل ارائه می‌دهد و شعبه فیزیکی ندارد.</p><h2>مزایای ترابانک‌ها</h2><ul><li>افتتاح حساب بدون مراجعه حضوری</li><li>کارمزدهای کمتر</li><li>رابط کاربری ساده و مدرن</li><li>خدمات ۲۴ ساعته</li></ul><h2>ترابانک‌های فعال ایران</h2><p>بلوبانک (بانک سامان)، هپی بانک (بانک پاسارگاد)، بانکینو (بانک خاورمیانه) از محبوب‌ترین ترابانک‌های ایران هستند.</p>",
                MetaTitle = "ترابانک چیست؟ معرفی نئوبانک‌های ایران",
                MetaDescription = "آشنایی کامل با ترابانک‌ها و نئوبانک‌های ایران. بلوبانک، هپی، بانکینو و سایر ترابانک‌ها.",
                MetaKeywords = "ترابانک, نئوبانک, بلوبانک, هپی بانک, بانکینو",
                IsPublished = true, ViewCount = 2100, CreatedAt = d, PublishedAt = d, UpdatedAt = d
            },
            new BlogPost
            {
                Id = 4, Title = "وام بدون ضامن: واقعیت یا افسانه؟", Slug = "vam-bedone-zamen",
                Summary = "آیا واقعاً می‌توان بدون ضامن وام گرفت؟ بررسی شرایط و بانک‌هایی که وام بدون ضامن ارائه می‌دهند.",
                Content = "<h2>آیا وام بدون ضامن وجود دارد؟</h2><p>بله! با گسترش بانکداری دیجیتال و سیستم‌های اعتبارسنجی، برخی بانک‌ها و ترابانک‌ها وام‌های بدون ضامن ارائه می‌دهند.</p><h2>کدام بانک‌ها وام بدون ضامن می‌دهند؟</h2><ul><li>بلوبانک: تا ۵۰ میلیون تومان</li><li>هپی بانک: تا ۴۰ میلیون تومان</li></ul><h2>شرایط</h2><p>داشتن سابقه حساب، گردش مالی مناسب و اعتبارسنجی مثبت از جمله شرایط اصلی هستند.</p>",
                MetaTitle = "وام بدون ضامن - کدام بانک‌ها ارائه می‌دهند؟",
                MetaDescription = "لیست بانک‌ها و ترابانک‌هایی که وام بدون ضامن ارائه می‌دهند. شرایط و مبالغ.",
                MetaKeywords = "وام بدون ضامن, وام آنلاین, بلوبانک, هپی بانک",
                IsPublished = true, ViewCount = 3400, CreatedAt = d, PublishedAt = d, UpdatedAt = d
            },
            new BlogPost
            {
                Id = 5, Title = "چگونه اعتبارسنجی بانکی خود را بهبود دهیم؟", Slug = "behbood-etebarsanji",
                Summary = "راهکارهای عملی برای بهبود امتیاز اعتبارسنجی و افزایش شانس دریافت وام.",
                Content = "<h2>اعتبارسنجی بانکی چیست؟</h2><p>اعتبارسنجی فرآیندی است که بانک‌ها برای ارزیابی توانایی بازپرداخت متقاضیان وام استفاده می‌کنند.</p><h2>عوامل مؤثر</h2><ul><li>سابقه پرداخت اقساط</li><li>میزان بدهی‌های فعال</li><li>گردش حساب بانکی</li><li>سابقه چک برگشتی</li></ul><h2>راهکارهای بهبود</h2><p>پرداخت به‌موقع اقساط، کاهش بدهی‌ها و داشتن گردش حساب منظم از مهم‌ترین راهکارها هستند.</p>",
                MetaTitle = "بهبود اعتبارسنجی بانکی - راهکارهای عملی",
                MetaDescription = "راهنمای بهبود امتیاز اعتبارسنجی بانکی برای افزایش شانس تأیید وام.",
                MetaKeywords = "اعتبارسنجی بانکی, امتیاز اعتبار, وام",
                IsPublished = true, ViewCount = 1800, CreatedAt = d, PublishedAt = d, UpdatedAt = d
            },
            new BlogPost
            {
                Id = 6, Title = "وام خودرو ۱۴۰۴: شرایط و بانک‌های ارائه‌دهنده", Slug = "vam-khodro-1404",
                Summary = "بررسی شرایط وام خودرو در سال ۱۴۰۴ و مقایسه بین بانک‌های مختلف.",
                Content = "<h2>وام خودرو</h2><p>وام خودرو یکی از محبوب‌ترین تسهیلات بانکی است. در سال ۱۴۰۴ بانک‌های مختلف شرایط متنوعی برای خرید خودرو ارائه می‌دهند.</p><p>بیشتر بانک‌ها وام خودرو با نرخ ۲۳ درصد و بازپرداخت ۶۰ ماهه ارائه می‌دهند. خودرو خریداری‌شده به عنوان وثیقه در رهن بانک قرار می‌گیرد.</p>",
                MetaTitle = "وام خودرو ۱۴۰۴ - شرایط و بانک‌ها",
                MetaDescription = "شرایط وام خودرو سال ۱۴۰۴. مقایسه نرخ سود و شرایط بانک‌های مختلف.",
                MetaKeywords = "وام خودرو, وام خودرو ۱۴۰۴, خرید خودرو",
                IsPublished = false, ViewCount = 0, CreatedAt = d, UpdatedAt = d
            },
            new BlogPost
            {
                Id = 7, Title = "نکات مهم قبل از دریافت وام مسکن", Slug = "nokat-ghabl-vam-maskan",
                Summary = "نکات کلیدی که قبل از اقدام به دریافت وام مسکن باید بدانید.",
                Content = "<h2>قبل از وام مسکن</h2><p>دریافت وام مسکن تصمیم مهمی است. قبل از اقدام، نکات زیر را در نظر بگیرید...</p><p>این مقاله در حال تکمیل است.</p>",
                MetaTitle = "نکات مهم قبل از دریافت وام مسکن",
                MetaDescription = "نکات کلیدی و مهم قبل از دریافت وام مسکن.",
                MetaKeywords = "وام مسکن, نکات وام مسکن",
                IsPublished = false, ViewCount = 0, CreatedAt = d, UpdatedAt = d
            },
            new BlogPost
            {
                Id = 8, Title = "آینده بانکداری دیجیتال در ایران", Slug = "ayandeh-bankdari-digital",
                Summary = "نگاهی به آینده بانکداری دیجیتال و ترابانک‌ها در ایران.",
                Content = "<h2>بانکداری دیجیتال</h2><p>با رشد سریع فناوری و تغییر عادت‌های مصرف‌کنندگان، بانکداری دیجیتال در ایران با سرعت بالایی در حال رشد است...</p><p>این مقاله در حال تکمیل است.</p>",
                MetaTitle = "آینده بانکداری دیجیتال ایران",
                MetaDescription = "بررسی آینده بانکداری دیجیتال و ترابانک‌ها در ایران.",
                MetaKeywords = "بانکداری دیجیتال, ترابانک, نئوبانک, آینده بانکداری",
                IsPublished = false, ViewCount = 0, CreatedAt = d, UpdatedAt = d
            }
        );

    }
}
