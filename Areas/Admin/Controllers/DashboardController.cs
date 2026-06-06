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
        private readonly ApplicationDbContext _context;

        public DashboardController(IBookService bookService, ApplicationDbContext context)
        {
            _bookService = bookService;
            _context = context;
        }

        //глав. стр. админ. панели
        public async Task<IActionResult> Index()
        {
            //сбор статистики для отобр. на дашборде
            ViewBag.TotalBooks = await _context.Books.CountAsync();
            ViewBag.TotalAuthors = await _context.Authors.CountAsync();
            ViewBag.TotalCategories = await _context.Categories.CountAsync();
            ViewBag.TotalPublishers = await _context.Publishers.CountAsync();

            return View();
        }
    }
}