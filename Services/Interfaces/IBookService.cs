using BookStore.Models.Entities;
using BookStore.Models.ViewModels;

namespace BookStore.Services.Interfaces
{
    /// <summary>
    /// Сервис содержит всю бизнес-логику по работе с книгами
    /// </summary>
    public interface IBookService
    {
        // Методы для публичной части сайта
        Task<IEnumerable<Book>> GetAllBooksAsync();
        Task<Book?> GetBookByIdAsync(int id);
        Task<IEnumerable<Book>> GetBooksByCategoryAsync(int categoryId);
        Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm);

        /// <summary>
        /// Поиск + фильтрация книг
        /// </summary>
        Task<IEnumerable<Book>> SearchAndFilterAsync(
            string? searchTerm,
            int? categoryId,
            decimal? minPrice,
            decimal? maxPrice);

        // Методы для Админ-панели
        Task<IEnumerable<BookViewModel>> GetAllBooksForAdminAsync();
        Task<BookViewModel?> GetBookForEditAsync(int id);

        Task AddBookAsync(BookViewModel model);
        Task UpdateBookAsync(BookViewModel model);
        Task DeleteBookAsync(int id);

        // Старые методы для совместимости
        Task AddBookAsync(Book book);
        Task UpdateBookAsync(Book book);
    }
}