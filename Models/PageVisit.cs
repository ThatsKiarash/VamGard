using System.ComponentModel.DataAnnotations;

namespace VamYab.Models;

public class PageVisit
{
    public int Id { get; set; }

    [Required]
    [MaxLength(500)]
    public string Path { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? IpAddress { get; set; }

    [MaxLength(500)]
    public string? UserAgent { get; set; }

    public DateTime VisitedAt { get; set; } = DateTime.UtcNow;
}
