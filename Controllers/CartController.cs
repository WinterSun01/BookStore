using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookStore.Services.Interfaces;
using System.Security.Claims;

namespace BookStore.Controllers
{
    [Authorize]   // Только для авторизованных пользователей
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // GET: /Cart
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var items = await _cartService.GetCartItemsAsync(userId!);
            ViewBag.Total = await _cartService.GetCartTotalAsync(userId!);

            return View(items);
        }

        // POST: /Cart/Add/5
        [HttpPost]
        public async Task<IActionResult> Add(int bookId, int quantity = 1)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _cartService.AddToCartAsync(userId!, bookId, quantity);

            // Передаём сообщение на следующую страницу
            TempData["SuccessMessage"] = "Книга успешно добавлена в корзину ✓";

            // Возвращаемся обратно на страницу, откуда пришли
            return Redirect(Request.Headers["Referer"].ToString() ?? "/Book");
        }

        // POST: /Cart/Remove/5
        [HttpPost]
        public async Task<IActionResult> Remove(int bookId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _cartService.RemoveFromCartAsync(userId!, bookId);

            TempData["SuccessMessage"] = "Корзина обновлена ✓";

            return RedirectToAction(nameof(Index));
        }

        // POST: /Cart/UpdateQuantity
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int bookId, int quantity)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _cartService.UpdateQuantityAsync(userId!, bookId, quantity);

            TempData["SuccessMessage"] = "Корзина обновлена ✓";

            return RedirectToAction(nameof(Index));
        }

        // POST: /Cart/Clear
        [HttpPost]
        public async Task<IActionResult> Clear()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _cartService.ClearCartAsync(userId!);

            return RedirectToAction(nameof(Index));
        }
    }
}