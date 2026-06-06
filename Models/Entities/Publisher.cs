using System.ComponentModel.DataAnnotations;

namespace BookStore.Models.Entities
{
    public class Publisher
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
        public string? Country { get; set; }

        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}