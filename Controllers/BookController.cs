using BookStore.Data;
using BookStore.Services;
using BookStore.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookService _bookService;
        private readonly IReviewService _reviewService;
        private readonly ApplicationDbContext _context;
        private readonly IFavoriteService _favoriteService;

        public BookController(
            IBookService bookService,
            IReviewService reviewService,
            IFavoriteService favoriteService,
            ApplicationDbContext context)
        {
            _bookService = bookService;
            _reviewService = reviewService;
            _favoriteService = favoriteService;
            _context = context;
        }

        public async Task<IActionResult> Index(
    string? searchTerm,
    int? categoryId,
    decimal? minPrice,
    decimal? maxPrice,
    int page = 1)
        {
            const int pageSize = 12;

            var allBooks = await _bookService.SearchAndFilterAsync(searchTerm, categoryId, minPrice, maxPrice);
            var totalItems = allBooks.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            var booksOnPage = allBooks
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var ratings = new Dictionary<int, double>();
            foreach (var book in booksOnPage)
            {
                var avg = await _reviewService.GetAverageRatingAsync(book.Id);
                ratings[book.Id] = avg;
            }
            ViewBag.BookRatings = ratings;

            ViewBag.SearchTerm = searchTerm;
            ViewBag.CategoryId = categoryId;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.Categories = await _context.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.HasPreviousPage = page > 1;
            ViewBag.HasNextPage = page < totalPages;

            return View(booksOnPage);
        }

        public async Task<IActionResult> Details(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            var approvedReviews = await _reviewService.GetApprovedReviewsByBookAsync(id);
            var averageRating = await _reviewService.GetAverageRatingAsync(id);
            var reviewCount = await _reviewService.GetApprovedReviewCountAsync(id);

            ViewBag.ApprovedReviews = approvedReviews;
            ViewBag.AverageRating = averageRating;
            ViewBag.ReviewCount = reviewCount;

            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (userId != null)
                {
                    ViewBag.IsInFavorites = await _favoriteService.IsBookInFavoritesAsync(userId, id);
                }
            }

            return View(book);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToFavorites(int bookId)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            await _favoriteService.AddToFavoritesAsync(userId, bookId);
            return RedirectToAction("Details", new { id = bookId });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RemoveFromFavorites(int bookId)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            await _favoriteService.RemoveFromFavoritesAsync(userId, bookId);
            return RedirectToAction("Details", new { id = bookId });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddReview(int BookId, int Rating, string Comment)
        {
            if (!User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var review = new BookStore.Models.Entities.Review
            {
                BookId = BookId,
                UserId = userId,
                Rating = Rating,
                Comment = Comment
            };

            await _reviewService.AddReviewAsync(review);

            TempData["Success"] = "Ваш отзыв отправлен на модерацию. Спасибо!";
            return RedirectToAction("Details", new { id = BookId });
        }
    }
}