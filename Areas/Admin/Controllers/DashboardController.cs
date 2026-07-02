using BookStore.Data;
using BookStore.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly IBookService _bookService;
        private readonly IReviewService _reviewService;
        private readonly ApplicationDbContext _context;

        public DashboardController(
            IBookService bookService,
            IReviewService reviewService,
            ApplicationDbContext context)
        {
            _bookService = bookService;
            _reviewService = reviewService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalBooks = await _context.Books.CountAsync();
            ViewBag.TotalAuthors = await _context.Authors.CountAsync();
            ViewBag.TotalCategories = await _context.Categories.CountAsync();
            ViewBag.TotalPublishers = await _context.Publishers.CountAsync();
            ViewBag.TotalArticles = await _context.Articles.CountAsync();

            var pendingReviews = await _reviewService.GetPendingReviewsAsync();
            ViewBag.PendingReviewsCount = pendingReviews.Count;

            return View();
        }
    }
}