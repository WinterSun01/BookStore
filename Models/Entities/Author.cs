using System.ComponentModel.DataAnnotations;

namespace BookStore.Models.Entities
{
    public class Author
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        public string? Biography { get; set; }
        public string? PhotoUrl { get; set; }

        // Навигационное свойство
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}