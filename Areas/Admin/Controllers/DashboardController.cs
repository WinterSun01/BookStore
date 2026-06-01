using BookStore.Data;                    // ← Добавь эту строку
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
        private readonly ApplicationDbContext _context;   // ← Добавь эту строку

        public DashboardController(IBookService bookService, ApplicationDbContext context)
        {
            _bookService = bookService;
            _context = context;                           // ← Добавь эту строку
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalBooks = await _context.Books.CountAsync();
            ViewBag.TotalAuthors = await _context.Authors.CountAsync();
            ViewBag.TotalCategories = await _context.Categories.CountAsync();
            ViewBag.TotalPublishers = await _context.Publishers.CountAsync();

            return View();
        }
    }
}