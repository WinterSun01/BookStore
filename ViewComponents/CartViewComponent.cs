using Microsoft.AspNetCore.Mvc;
using BookStore.Services.Interfaces;
using System.Security.Claims;

namespace BookStore.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        private readonly ICartService _cartService;

        public CartViewComponent(ICartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = UserClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            int count = 0;

            if (!string.IsNullOrEmpty(userId))
            {
                count = await _cartService.GetCartItemsCountAsync(userId);
            }

            return View(count);
        }
    }
}