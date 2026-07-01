using System.ComponentModel.DataAnnotations;

namespace BookStore.Models.Entities;

public class Favorite
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;

    public int BookId { get; set; }
    public Book Book { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}