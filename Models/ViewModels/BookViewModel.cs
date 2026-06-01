using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace BookStore.Models.ViewModels
{
    public class BookViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Название обязательно")]
        [Display(Name = "Название книги")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Описание")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Цена")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Display(Name = "Количество на складе")]
        public int Stock { get; set; }

        [Display(Name = "Обложка (URL)")]
        public string? ImageUrl { get; set; }

        // ← Добавь эту строку
        public string? CurrentImageUrl { get; set; }

        [Required]
        [Display(Name = "Автор")]
        public int AuthorId { get; set; }

        [Required]
        [Display(Name = "Категория")]
        public int CategoryId { get; set; }

        [Required]
        [Display(Name = "Издательство")]
        public int PublisherId { get; set; }

        public IFormFile? ImageFile { get; set; }
    }
}