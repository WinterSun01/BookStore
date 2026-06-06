using BookStore.Models.Entities;
using BookStore.Services.Interfaces;
using BookStore.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Services
{
    public class CartService : ICartService
    {
        private readonly IShoppingCartRepository _cartRepository;
        private readonly IBookRepository _bookRepository;

        public CartService(IShoppingCartRepository cartRepository, IBookRepository bookRepository)
        {
            _cartRepository = cartRepository;
            _bookRepository = bookRepository;
        }

        public async Task AddToCartAsync(string userId, int bookId, int quantity = 1)
        {
            await _cartRepository.AddOrUpdateAsync(userId, bookId, quantity);
        }

        public async Task RemoveFromCartAsync(string userId, int bookId)
        {
            await _cartRepository.RemoveAsync(userId, bookId);
        }

        public async Task UpdateQuantityAsync(string userId, int bookId, int quantity)
        {
            await _cartRepository.UpdateQuantityAsync(userId, bookId, quantity);
        }

        public async Task ClearCartAsync(string userId)
        {
            await _cartRepository.ClearAsync(userId);
        }

        public async Task<IEnumerable<ShoppingCartItem>> GetCartItemsAsync(string userId)
        {
            return await _cartRepository.GetCartItemsAsync(userId);
        }

        public async Task<int> GetCartItemsCountAsync(string userId)
        {
            return await _cartRepository.GetCartItemsCountAsync(userId);
        }

        public async Task<decimal> GetCartTotalAsync(string userId)
        {
            return await _cartRepository.GetCartTotalAsync(userId);
        }
    }
}