using System.ComponentModel.DataAnnotations;

namespace BookStore.Models.Entities
{
    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Название книги обязательно")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }           // Обложка книги
        public int Stock { get; set; } = 0;             // Количество на складе

        public int AuthorId { get; set; }
        public Author Author { get; set; } = null!;     // Навигационное свойство

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public int PublisherId { get; set; }
        public Publisher Publisher { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}