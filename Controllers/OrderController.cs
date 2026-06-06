using BookStore.Data;
using BookStore.Models.Entities;
using BookStore.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookStore.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ICartService _cartService;
        private readonly ApplicationDbContext _context;

        public OrderController(ICartService cartService, ApplicationDbContext context)
        {
            _cartService = cartService;
            _context = context;
        }

        //форма оформ. заказа на основе тек. корзины
        public async Task<IActionResult> Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartItems = await _cartService.GetCartItemsAsync(userId!);

            if (!cartItems.Any())
                return RedirectToAction("Index", "Cart");

            ViewBag.CartItems = cartItems;
            ViewBag.Total = await _cartService.GetCartTotalAsync(userId!);

            var order = new Order
            {
                UserId = userId!,
                OrderDate = DateTime.UtcNow,
                Status = "Pending",
                TotalAmount = ViewBag.Total,
                ShippingAddress = ""
            };

            return View(order);
        }

        //ист. заказов тек. польз.
        public async Task<IActionResult> MyOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Book)
                        .ThenInclude(b => b.Author)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        //созд. и сохр. заказа
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order order)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartItems = await _cartService.GetCartItemsAsync(userId!);

            if (!cartItems.Any())
                return RedirectToAction("Index", "Cart");

            if (ModelState.IsValid)
            {
                order.UserId = userId!;
                order.OrderDate = DateTime.UtcNow;
                order.TotalAmount = await _cartService.GetCartTotalAsync(userId!);

                if (order.PaymentMethod == "Card")
                {
                    order.Status = "Paid";
                }
                else if (order.PaymentMethod == "Cash")
                {
                    order.Status = "AwaitingPayment";
                }
                else
                {
                    order.Status = "Pending";
                }

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                foreach (var cartItem in cartItems)
                {
                    _context.OrderItems.Add(new OrderItem
                    {
                        OrderId = order.Id,
                        BookId = cartItem.BookId,
                        Quantity = cartItem.Quantity,
                        PricePerUnit = cartItem.Book.Price
                    });
                }

                await _context.SaveChangesAsync();

                await _cartService.ClearCartAsync(userId!);

                TempData["Success"] = "✅ Заказ успешно оформлен!";
                return RedirectToAction("MyOrders", "Order");
            }

            ViewBag.CartItems = cartItems;
            return View(order);
        }
    }
}