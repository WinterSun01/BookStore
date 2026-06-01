using Microsoft.AspNetCore.Mvc;
using BookStore.Services.Interfaces;
using BookStore.Data;                    // Для ViewBag.Categories
using Microsoft.EntityFrameworkCore;

namespace BookStore.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookService _bookService;
        private readonly ApplicationDbContext _context;   // Для списка категорий в фильтре

        public BookController(IBookService bookService, ApplicationDbContext context)
        {
            _bookService = bookService;
            _context = context;
        }

        // GET: /Book (с поиском, фильтрами и пагинацией)
        public async Task<IActionResult> Index(
            string? searchTerm,
            int? categoryId,
            decimal? minPrice,
            decimal? maxPrice,
            int page = 1)
        {
            const int pageSize = 12;

            // Получаем все книги по фильтрам
            var allBooks = await _bookService.SearchAndFilterAsync(searchTerm, categoryId, minPrice, maxPrice);

            var totalItems = allBooks.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            // Берём только нужную страницу
            var booksOnPage = allBooks
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Передаём данные для фильтров
            ViewBag.SearchTerm = searchTerm;
            ViewBag.CategoryId = categoryId;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;

            ViewBag.Categories = await _context.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();

            // Данные для пагинации (передаём в View)
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.HasPreviousPage = page > 1;
            ViewBag.HasNextPage = page < totalPages;

            return View(booksOnPage);
        }

        // GET: /Book/Details/5
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