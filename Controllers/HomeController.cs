using Microsoft.AspNetCore.Mvc;
using BookStore.Services.Interfaces;

namespace BookStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBookService _bookService;
        private readonly IReviewService _reviewService;

        public HomeController(IBookService bookService, IReviewService reviewService)
        {
            _bookService = bookService;
            _reviewService = reviewService;
        }

        public async Task<IActionResult> Index()
        {
            var books = await _bookService.GetAllBooksAsync();

            var ratings = new Dictionary<int, double>();
            foreach (var book in books)
            {
                var avgRating = await _reviewService.GetAverageRatingAsync(book.Id);
                ratings[book.Id] = avgRating;
            }
            ViewBag.BookRatings = ratings;

            return View(books);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}