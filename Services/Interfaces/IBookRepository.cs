using BookStore.Models.Entities;

namespace BookStore.Repositories.Interfaces
{
    // Интерфейс репозитория отвечает только за работу с БД
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllAsync();
        Task<Book?> GetByIdAsync(int id);
        Task<IEnumerable<Book>> GetByCategoryAsync(int categoryId);
        Task<IEnumerable<Book>> SearchAsync(string searchTerm);
        Task AddAsync(Book book);
        Task UpdateAsync(Book book);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}