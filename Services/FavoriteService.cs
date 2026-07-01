using BookStore.Data;
using BookStore.Models.Entities;
using BookStore.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Services;

public class FavoriteService : IFavoriteService
{
    private readonly ApplicationDbContext _context;

    public FavoriteService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> AddToFavoritesAsync(string userId, int bookId)
    {
        var exists = await _context.Favorites
            .AnyAsync(f => f.UserId == userId && f.BookId == bookId);

        if (exists)
            return false;

        var favorite = new Favorite
        {
            UserId = userId,
            BookId = bookId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Favorites.Add(favorite);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveFromFavoritesAsync(string userId, int bookId)
    {
        var favorite = await _context.Favorites
            .FirstOrDefaultAsync(f => f.UserId == userId && f.BookId == bookId);

        if (favorite == null)
            return false;

        _context.Favorites.Remove(favorite);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Book>> GetUserFavoritesAsync(string userId)
    {
        return await _context.Favorites
            .Where(f => f.UserId == userId)
            .Include(f => f.Book)
                .ThenInclude(b => b.Author)
            .Select(f => f.Book)
            .ToListAsync();
    }

    public async Task<bool> IsBookInFavoritesAsync(string userId, int bookId)
    {
        return await _context.Favorites
            .AnyAsync(f => f.UserId == userId && f.BookId == bookId);
    }
}