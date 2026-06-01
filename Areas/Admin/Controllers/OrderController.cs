using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookStore.Data;
using BookStore.Models.Entities;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/Order
        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                .Include(o => o.User)           // ← Вот это важно!
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Book)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            // Простая статистика
            ViewBag.TotalOrders = orders.Count;
            ViewBag.TotalRevenue = orders.Sum(o => o.TotalAmount);
            ViewBag.PaidOrders = orders.Count(o => o.Status == "Paid");

            return View(orders);
        }

        // GET: /Admin/Order/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Book)
                        .ThenInclude(b => b.Author)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            return View(order);
        }

        // POST: Изменение статуса
        [HttpPost]
        public async Task<IActionResult> ChangeStatus(int id, string status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                order.Status = status;
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Статус заказа №{id} изменён на '{status}'";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}