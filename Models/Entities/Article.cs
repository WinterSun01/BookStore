using System.ComponentModel.DataAnnotations;

namespace BookStore.Models.Entities;

public class Article
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? ShortDescription { get; set; }

    public string Content { get; set; } = string.Empty; // HTML-контент

    public string? ImageUrl { get; set; }

    [MaxLength(200)]
    public string Slug { get; set; } = string.Empty; // для красивых URL

    public DateTime PublishedAt { get; set; } = DateTime.UtcNow;
    public bool IsPublished { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<Book> Books { get; set; } = new List<Book>();
}