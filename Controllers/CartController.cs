using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookStore.Services.Interfaces;
using System.Security.Claims;

namespace BookStore.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        //содержимое корзины тек. польз.
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var items = await _cartService.GetCartItemsAsync(userId!);
            ViewBag.Total = await _cartService.GetCartTotalAsync(userId!);
            return View(items);
        }

        //доб. книгу в корзину
        [HttpPost]
        public async Task<IActionResult> Add(int bookId, int quantity = 1)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _cartService.AddToCartAsync(userId!, bookId, quantity);

            TempData["SuccessMessage"] = "Книга успешно добавлена в корзину ✓";

            return Redirect(Request.Headers["Referer"].ToString() ?? "/Book");
        }

        //удал. книгу из корзины
        [HttpPost]
        public async Task<IActionResult> Remove(int bookId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _cartService.RemoveFromCartAsync(userId!, bookId);

            TempData["SuccessMessage"] = "Корзина обновлена ✓";
            return RedirectToAction(nameof(Index));
        }

        //изм. кол-во товара в корзине
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int bookId, int quantity)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _cartService.UpdateQuantityAsync(userId!, bookId, quantity);

            TempData["SuccessMessage"] = "Корзина обновлена ✓";
            return RedirectToAction(nameof(Index));
        }

        //очищ. всю корзину
        [HttpPost]
        public async Task<IActionResult> Clear()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _cartService.ClearCartAsync(userId!);
            return RedirectToAction(nameof(Index));
        }
    }
}