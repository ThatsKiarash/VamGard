using System.ComponentModel.DataAnnotations;

namespace VamYab.Models;

public class NewsletterSubscriber
{
    public int Id { get; set; }

    [Required(ErrorMessage = "ایمیل الزامی است")]
    [EmailAddress(ErrorMessage = "ایمیل معتبر وارد کنید")]
    [MaxLength(250)]
    [Display(Name = "ایمیل")]
    public string Email { get; set; } = string.Empty;

    [MaxLength(100)]
    [Display(Name = "نام")]
    public string? Name { get; set; }

    [Display(Name = "فعال")]
    public bool IsActive { get; set; } = true;

    public DateTime SubscribedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(50)]
    public string? IpAddress { get; set; }
}
