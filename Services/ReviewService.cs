using BookStore.Data;
using BookStore.Models.Entities;
using BookStore.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Services;

public class ReviewService : IReviewService
{
    private readonly ApplicationDbContext _context;

    public ReviewService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Review?> GetByIdAsync(int id)
    {
        return await _context.Reviews
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<List<Review>> GetApprovedReviewsByBookAsync(int bookId)
    {
        return await _context.Reviews
            .Where(r => r.BookId == bookId && r.IsApproved)
            .Include(r => r.User)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Review>> GetPendingReviewsAsync()
    {
        return await _context.Reviews
            .Where(r => !r.IsApproved)
            .Include(r => r.User)
            .Include(r => r.Book)
            .OrderBy(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> HasUserAlreadyReviewedAsync(string userId, int bookId)
    {
        return await _context.Reviews
            .AnyAsync(r => r.UserId == userId && r.BookId == bookId);
    }

    public async Task<Review> AddReviewAsync(Review review)
    {
        review.IsApproved = false;
        review.CreatedAt = DateTime.UtcNow;

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        return review;
    }

    public async Task<bool> ApproveReviewAsync(int reviewId)
    {
        var review = await _context.Reviews.FindAsync(reviewId);
        if (review == null) return false;

        review.IsApproved = true;
        review.ApprovedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteReviewAsync(int reviewId)
    {
        var review = await _context.Reviews.FindAsync(reviewId);
        if (review == null) return false;

        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<double> GetAverageRatingAsync(int bookId)
    {
        var reviews = await _context.Reviews
            .Where(r => r.BookId == bookId && r.IsApproved)
            .ToListAsync();

        if (!reviews.Any()) return 0;

        return Math.Round(reviews.Average(r => r.Rating), 1);
    }

    public async Task<int> GetApprovedReviewCountAsync(int bookId)
    {
        return await _context.Reviews
            .CountAsync(r => r.BookId == bookId && r.IsApproved);
    }
}