using BookStore.Models.Entities;

namespace BookStore.Services.Interfaces
{
    public interface ICartService
    {
        Task AddToCartAsync(string userId, int bookId, int quantity = 1);
        Task RemoveFromCartAsync(string userId, int bookId);
        Task UpdateQuantityAsync(string userId, int bookId, int quantity);
        Task ClearCartAsync(string userId);
        Task<IEnumerable<ShoppingCartItem>> GetCartItemsAsync(string userId);
        Task<int> GetCartItemsCountAsync(string userId);
        Task<decimal> GetCartTotalAsync(string userId);
    }
}