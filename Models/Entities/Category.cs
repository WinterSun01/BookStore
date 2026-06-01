using System.ComponentModel.DataAnnotations;

namespace BookStore.Models.Entities
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        // Навигационное свойство
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}