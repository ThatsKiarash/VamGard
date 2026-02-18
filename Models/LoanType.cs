using System.ComponentModel.DataAnnotations;

namespace VamYab.Models;

public class LoanType
{
    public int Id { get; set; }

    [Required(ErrorMessage = "نام نوع وام الزامی است")]
    [MaxLength(150)]
    [Display(Name = "نام نوع وام")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    [Display(Name = "اسلاگ")]
    public string Slug { get; set; } = string.Empty;

    [Display(Name = "توضیحات")]
    public string? Description { get; set; }

    [MaxLength(100)]
    [Display(Name = "آیکون (CSS Class)")]
    public string? IconClass { get; set; }

    [Display(Name = "فعال")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "ترتیب نمایش")]
    public int DisplayOrder { get; set; }

    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
}
