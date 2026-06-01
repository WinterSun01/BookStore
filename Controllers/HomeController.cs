using Microsoft.AspNetCore.Mvc;
using BookStore.Services.Interfaces;

namespace BookStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBookService _bookService;

        public HomeController(IBookService bookService)
        {
            _bookService = bookService;
        }

        public async Task<IActionResult> Index()
        {
            // Получаем книги для главной страницы
            var books = await _bookService.GetAllBooksAsync();
            return View(books);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}