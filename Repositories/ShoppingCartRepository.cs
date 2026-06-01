using Microsoft.EntityFrameworkCore;
using BookStore.Data;
using BookStore.Models.Entities;
using BookStore.Repositories.Interfaces;

namespace BookStore.Repositories
{
    public class ShoppingCartRepository : IShoppingCartRepository
    {
        private readonly ApplicationDbContext _context;

        public ShoppingCartRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddOrUpdateAsync(string userId, int bookId, int quantity)
        {
            var existingItem = await _context.ShoppingCartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.BookId == bookId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                await _context.ShoppingCartItems.AddAsync(new ShoppingCartItem
                {
                    UserId = userId,
                    BookId = bookId,
                    Quantity = quantity
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(string userId, int bookId)
        {
            var item = await _context.ShoppingCartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.BookId == bookId);

            if (item != null)
            {
                _context.ShoppingCartItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateQuantityAsync(string userId, int bookId, int quantity)
        {
            var item = await _context.ShoppingCartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.BookId == bookId);

            if (item != null)
            {
                item.Quantity = quantity;
                await _context.SaveChangesAsync();
            }
        }

        public async Task ClearAsync(string userId)
        {
            var items = await _context.ShoppingCartItems
                .Where(c => c.UserId == userId)
                .ToListAsync();

            _context.ShoppingCartItems.RemoveRange(items);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ShoppingCartItem>> GetCartItemsAsync(string userId)
        {
            return await _context.ShoppingCartItems
                .Where(c => c.UserId == userId)
                .Include(c => c.Book)
                .ThenInclude(b => b.Author)
                .ToListAsync();
        }

        public async Task<int> GetCartItemsCountAsync(string userId)
        {
            return await _context.ShoppingCartItems
                .Where(c => c.UserId == userId)
                .SumAsync(c => c.Quantity);
        }

        public async Task<decimal> GetCartTotalAsync(string userId)
        {
            return await _context.ShoppingCartItems
                .Where(c => c.UserId == userId)
                .Include(c => c.Book)
                .SumAsync(c => c.Quantity * c.Book.Price);
        }
    }
}