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

        //список всех заказов с осн. инф.
        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Book)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            ViewBag.TotalOrders = orders.Count;
            ViewBag.TotalRevenue = orders.Sum(o => o.TotalAmount);
            ViewBag.PaidOrders = orders.Count(o => o.Status == "Paid");

            return View(orders);
        }

        //детал. инф. по конкр. заказу
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

        //быстрая смена статуса заказа
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