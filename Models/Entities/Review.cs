using System.ComponentModel.DataAnnotations;

namespace BookStore.Models.Entities;

public class Review
{
    public int Id { get; set; }

    public int BookId { get; set; }
    public Book Book { get; set; } = null!;

    public string UserId { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;

    [Range(1, 5)]
    public int Rating { get; set; }

    [Required]
    [StringLength(1000)]
    public string Comment { get; set; } = string.Empty;

    public bool IsApproved { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ApprovedAt { get; set; }
}