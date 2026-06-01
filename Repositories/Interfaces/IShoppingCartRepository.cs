using BookStore.Models.Entities;

namespace BookStore.Repositories.Interfaces
{
    public interface IShoppingCartRepository
    {
        Task AddOrUpdateAsync(string userId, int bookId, int quantity);
        Task RemoveAsync(string userId, int bookId);
        Task UpdateQuantityAsync(string userId, int bookId, int quantity);
        Task ClearAsync(string userId);
        Task<IEnumerable<ShoppingCartItem>> GetCartItemsAsync(string userId);
        Task<int> GetCartItemsCountAsync(string userId);
        Task<decimal> GetCartTotalAsync(string userId);
    }
}