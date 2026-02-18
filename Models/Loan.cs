using System.ComponentModel.DataAnnotations;

namespace VamYab.Models;

public class Loan
{
    public int Id { get; set; }

    [Required(ErrorMessage = "عنوان وام الزامی است")]
    [MaxLength(250)]
    [Display(Name = "عنوان وام")]
    public string Title { get; set; } = string.Empty;

    [MaxLength(300)]
    [Display(Name = "اسلاگ")]
    public string Slug { get; set; } = string.Empty;

    [Display(Name = "توضیحات کوتاه")]
    [MaxLength(500)]
    public string? ShortDescription { get; set; }

    [Display(Name = "توضیحات کامل")]
    public string? FullDescription { get; set; }

    [Display(Name = "نرخ سود (درصد)")]
    public decimal? InterestRate { get; set; }

    [Display(Name = "حداقل مبلغ (ریال)")]
    public long? MinAmount { get; set; }

    [Display(Name = "حداکثر مبلغ (ریال)")]
    public long? MaxAmount { get; set; }

    [Display(Name = "مدت بازپرداخت (ماه)")]
    public int? RepaymentMonths { get; set; }

    [Display(Name = "شرایط و مدارک")]
    public string? Requirements { get; set; }

    [Display(Name = "فعال بودن")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "ویژه")]
    public bool IsFeatured { get; set; }

    [Display(Name = "بانک")]
    [Required(ErrorMessage = "انتخاب بانک الزامی است")]
    public int BankId { get; set; }
    public Bank Bank { get; set; } = null!;

    [Display(Name = "نوع وام")]
    [Required(ErrorMessage = "انتخاب نوع وام الزامی است")]
    public int LoanTypeId { get; set; }
    public LoanType LoanType { get; set; } = null!;

    [Display(Name = "لینک ثبت‌نام در سایت بانک")]
    [MaxLength(500)]
    public string? ExternalUrl { get; set; }

    [Display(Name = "تعداد بازدید")]
    public int ViewCount { get; set; }

    [Display(Name = "متا تایتل")]
    [MaxLength(200)]
    public string? MetaTitle { get; set; }

    [Display(Name = "متا توضیحات")]
    [MaxLength(500)]
    public string? MetaDescription { get; set; }

    [Display(Name = "کلمات کلیدی")]
    [MaxLength(500)]
    public string? MetaKeywords { get; set; }

    [Display(Name = "تحلیل و بررسی")]
    public string? AnalysisContent { get; set; }

    [Display(Name = "ماشین‌حساب فعال")]
    public bool HasCalculator { get; set; } = true;

    [Display(Name = "حداقل مدت بازپرداخت (ماه)")]
    public int? CalcMinMonths { get; set; }

    [Display(Name = "حداکثر مدت بازپرداخت (ماه)")]
    public int? CalcMaxMonths { get; set; }

    [Display(Name = "گام تغییر مدت (ماه)")]
    public int? CalcMonthStep { get; set; }

    [Display(Name = "سود قابل تغییر در ماشین‌حساب")]
    public bool CalcAdjustableRate { get; set; }

    [Display(Name = "حداقل نرخ سود ماشین‌حساب")]
    public decimal? CalcMinRate { get; set; }

    [Display(Name = "حداکثر نرخ سود ماشین‌حساب")]
    public decimal? CalcMaxRate { get; set; }

    [MaxLength(500)]
    [Display(Name = "شناسه مقالات مرتبط")]
    public string? RelatedArticleIds { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
