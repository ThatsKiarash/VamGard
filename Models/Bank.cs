using System.ComponentModel.DataAnnotations;

namespace VamYab.Models;

public class Bank
{
    public int Id { get; set; }

    [Required(ErrorMessage = "نام بانک الزامی است")]
    [MaxLength(100)]
    [Display(Name = "نام بانک")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    [Display(Name = "اسلاگ")]
    public string Slug { get; set; } = string.Empty;

    [MaxLength(500)]
    [Display(Name = "لوگو (URL)")]
    public string? LogoUrl { get; set; }

    [MaxLength(300)]
    [Display(Name = "وب‌سایت")]
    public string? Website { get; set; }

    [Display(Name = "توضیحات")]
    public string? Description { get; set; }

    [Display(Name = "تاریخچه")]
    public string? History { get; set; }

    [Display(Name = "سال تأسیس")]
    public int? FoundedYear { get; set; }

    [Display(Name = "تعداد شعب")]
    public int? BranchCount { get; set; }

    [MaxLength(100)]
    [Display(Name = "شهر مرکزی")]
    public string? HeadquartersCity { get; set; }

    [Display(Name = "عرض جغرافیایی")]
    public double? Latitude { get; set; }

    [Display(Name = "طول جغرافیایی")]
    public double? Longitude { get; set; }

    [MaxLength(20)]
    [Display(Name = "نوع")]
    public string BankType { get; set; } = "bank";

    [Display(Name = "بانک مادر")]
    public int? ParentBankId { get; set; }

    [MaxLength(100)]
    [Display(Name = "نوع مالکیت")]
    public string? OwnershipType { get; set; }

    [Display(Name = "فعال")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "ترتیب نمایش")]
    public int DisplayOrder { get; set; }

    [MaxLength(500)]
    [Display(Name = "آدرس دفتر مرکزی")]
    public string? Address { get; set; }

    [MaxLength(20)]
    [Display(Name = "شماره تلفن")]
    public string? PhoneNumber { get; set; }

    [MaxLength(100)]
    [Display(Name = "ایمیل")]
    public string? Email { get; set; }

    [Display(Name = "لیست شعب (JSON)")]
    public string? BranchesJson { get; set; }

    [Display(Name = "متا تایتل")]
    [MaxLength(200)]
    public string? MetaTitle { get; set; }

    [Display(Name = "متا توضیحات")]
    [MaxLength(500)]
    public string? MetaDescription { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
}
