using BookStore.Models.Entities;

namespace BookStore.Services.Interfaces;

public interface IReviewService
{
    Task<Review?> GetByIdAsync(int id);

    Task<List<Review>> GetApprovedReviewsByBookAsync(int bookId);

    Task<List<Review>> GetPendingReviewsAsync();

    Task<bool> HasUserAlreadyReviewedAsync(string userId, int bookId);

    Task<Review> AddReviewAsync(Review review);

    Task<bool> ApproveReviewAsync(int reviewId);

    Task<bool> DeleteReviewAsync(int reviewId);

    Task<double> GetAverageRatingAsync(int bookId);

    Task<int> GetApprovedReviewCountAsync(int bookId);
}