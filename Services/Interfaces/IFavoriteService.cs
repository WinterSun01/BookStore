using BookStore.Models.Entities;

namespace BookStore.Services.Interfaces;

public interface IFavoriteService
{
    Task<bool> AddToFavoritesAsync(string userId, int bookId);
    Task<bool> RemoveFromFavoritesAsync(string userId, int bookId);
    Task<List<Book>> GetUserFavoritesAsync(string userId);
    Task<bool> IsBookInFavoritesAsync(string userId, int bookId);
}