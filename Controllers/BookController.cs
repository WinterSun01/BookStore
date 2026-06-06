using Microsoft.AspNetCore.Mvc;
using BookStore.Services.Interfaces;
using BookStore.Data;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookService _bookService;
        private readonly ApplicationDbContext _context;

        public BookController(IBookService bookService, ApplicationDbContext context)
        {
            _bookService = bookService;
            _context = context;
        }

        //каталог книг с поиском, фильтрами и пагинацией
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

        //детальн. стр. книги
        public async Task<IActionResult> Details(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }
    }
}