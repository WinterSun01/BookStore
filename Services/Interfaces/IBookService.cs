using BookStore.Models.Entities;
using BookStore.Models.ViewModels;

namespace BookStore.Services.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<Book>> GetAllBooksAsync();
        Task<Book?> GetBookByIdAsync(int id);
        Task<IEnumerable<Book>> GetBooksByCategoryAsync(int categoryId);
        Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm);

        Task<IEnumerable<Book>> SearchAndFilterAsync(
            string? searchTerm,
            int? categoryId,
            decimal? minPrice,
            decimal? maxPrice);

        Task<IEnumerable<BookViewModel>> GetAllBooksForAdminAsync();
        Task<BookViewModel?> GetBookForEditAsync(int id);

        Task AddBookAsync(BookViewModel model);
        Task UpdateBookAsync(BookViewModel model);
        Task DeleteBookAsync(int id);

        Task AddBookAsync(Book book);
        Task UpdateBookAsync(Book book);
    }
}