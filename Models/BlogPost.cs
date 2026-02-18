using System.ComponentModel.DataAnnotations;

namespace VamYab.Models;

public class BlogPost
{
    public int Id { get; set; }

    [Required(ErrorMessage = "عنوان الزامی است")]
    [MaxLength(300)]
    [Display(Name = "عنوان")]
    public string Title { get; set; } = string.Empty;

    [MaxLength(400)]
    [Display(Name = "اسلاگ")]
    public string Slug { get; set; } = string.Empty;

    [MaxLength(500)]
    [Display(Name = "خلاصه")]
    public string? Summary { get; set; }

    [Required(ErrorMessage = "محتوا الزامی است")]
    [Display(Name = "محتوا")]
    public string Content { get; set; } = string.Empty;

    [MaxLength(500)]
    [Display(Name = "تصویر کاور")]
    public string? CoverImageUrl { get; set; }

    [MaxLength(200)]
    [Display(Name = "متا تایتل")]
    public string? MetaTitle { get; set; }

    [MaxLength(500)]
    [Display(Name = "متا توضیحات")]
    public string? MetaDescription { get; set; }

    [MaxLength(500)]
    [Display(Name = "کلمات کلیدی")]
    public string? MetaKeywords { get; set; }

    [Display(Name = "منتشر شده")]
    public bool IsPublished { get; set; }

    [Display(Name = "تعداد بازدید")]
    public int ViewCount { get; set; }

    [MaxLength(200)]
    [Display(Name = "دسته‌بندی")]
    public string? Category { get; set; }

    [MaxLength(500)]
    [Display(Name = "برچسب‌ها")]
    public string? Tags { get; set; }

    [Display(Name = "بانک مرتبط")]
    public int? RelatedBankId { get; set; }

    [Display(Name = "وام مرتبط")]
    public int? RelatedLoanId { get; set; }

    [MaxLength(500)]
    [Display(Name = "شناسه وام‌های مرتبط")]
    public string? RelatedLoanIds { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Display(Name = "تاریخ انتشار")]
    public DateTime? PublishedAt { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
